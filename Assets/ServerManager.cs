using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class ServerManager : MonoBehaviour
{
    private Process pyServerProcess;

    public async Task<bool> StartServer()
    {
        if (AppState.serverState == AppState.ServerState.Ready) { return true; }
        AppState.serverState = AppState.ServerState.Starting;
        try
        {
            string pythonPath = GetPythonPath();
            string scriptPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "PyPutty", "main_server.py");

            if (!File.Exists(scriptPath))
            {
                Console.LogError($"Python скрипт не найден: {scriptPath}.");
                return false;
            }
            var startInfo = new ProcessStartInfo
            {
                FileName = pythonPath,
                Arguments = $"\"{scriptPath}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,  
                RedirectStandardError = true
            };
            pyServerProcess = new Process { StartInfo = startInfo };
            pyServerProcess.Start();
            // открытие файла main_server.py завершено...
            // тестирование сервера
            bool success = await WaitForServerStarting();
            if (success)
            {
                AppState.serverState = AppState.ServerState.Ready;
                Console.Log("Сервер запущен");
                return true;
            }
            else
            {
                AppState.serverState = AppState.ServerState.Error;
                return false;
            }
        }
        catch (Exception e)
        {
            AppState.serverState = AppState.ServerState.Error;
            Console.LogError($"Неожиданная ошибка при запуске сервера: {e.Message}.");
            await StopServer();
            return false;
        }
    }
    private async Task<bool> WaitForServerStarting()
    {
        int maxTimeWait = 5000;
        int elapsed = 0;

        while(elapsed < maxTimeWait)
        {
            if (pyServerProcess.HasExited && pyServerProcess.ExitCode != 0)
            {
                string error = await pyServerProcess.StandardError.ReadToEndAsync();
                Console.LogError($"Сервер завершился с ошибкой: {error}.");
                return false;
            }
            if (await IsServerReady()) // попытка подключения тестового клиента
            {
                return true;
            }
            await Task.Delay(100);
            elapsed += 100;
        }
        Console.LogError("Сервер не запустился за отведенное время");
        await StopServer();
        return false;
    }
    private async Task<bool> IsServerReady()
    {
        // создание тестового временного клиента для проверки подключения
        TcpClient testClient = null;
        try
        {
            testClient = new TcpClient();
            var connectTask = testClient.ConnectAsync("localhost", 8080);
            var timeoutTask = Task.Delay(500);

            var completedTask = await Task.WhenAny(connectTask, timeoutTask);
            if (completedTask == connectTask && testClient.Connected)
            {
                return true;
            }
            return false;
        }
        catch 
        { 
            return false; 
        }
        finally
        {
            if (testClient != null)
            {
                try
                {
                    testClient.Close();
                    testClient.Dispose();
                }
                catch { }
            }
        }
    }
    public async Task<bool> RestartServer()
    {
        await StopServer();
        await Task.Delay(1000);
        bool success = await StartServer();
        return success;
    }

    public async Task StopServer()
    {
        if (AppState.serverState == AppState.ServerState.Disconnected) { return; } 
        if (AppState.serverState == AppState.ServerState.Error)
        {
            Console.Log("Возникла ошибка сервер закрывается...");
        }
        AppState.serverState = AppState.ServerState.Disconnected;
        if (pyServerProcess != null && !pyServerProcess.HasExited)
        {
            await Task.Delay(1000);

            if (!pyServerProcess.HasExited)
            {
                pyServerProcess.Kill();
                await Task.Run(() => pyServerProcess.WaitForExit(3000));
            }

            pyServerProcess.Dispose();
            pyServerProcess = null;
        }
    }

    private void OnDestroy()
    {
        _ = StopServer();
    }
    private string GetPythonPath()
    {
        return "python";
    }
}
