using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Net.Sockets;
using System.Threading.Tasks;
using System;
using System.Globalization;
using System.Net;
using Unity.VisualScripting;
using UnityEngine.Rendering.UI;


public class Client : MonoBehaviour
{
    private Socket socket;
    public static event Action<float> OnDataFlow;
    private Queue<float> posBuffer = new Queue<float>();
    private float currentValue = 0f;
    public int bufferSize = 5;
    private string clientIP;
    private int clientPort;
    

    private void Start()
    {
        COMPanel.OnCOM_SET += HandleCommandSet;
        COMPanel.OnComDisconnect += HandleDisconnect;
        COMPanel.OnComConnect += HandleConnect;
        COMPanel.OnComStop += HandleStop;
    }
    public async Task<bool> Connect(string ip = "localhost", int port = 8080)
    {
        if (AppState.IsClientConnected) { return true; }
        try
        {
            clientIP = ip;
            clientPort = port;
            Disconnect();

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await socket.ConnectAsync("localhost", 8080); // асинхронное подключение
            AppState.IsClientConnected = true;
            Console.Log($"Подключение к {socket.RemoteEndPoint} установлено.");
            _ = TakeData(); // запуск фоновой задачи приема данных
            return true;
        }
        catch (Exception ex)
        {
            // обработка ошибок...
            Console.Log($"Не удалось подключится к {ex.Message}.");
            AppState.IsClientConnected = false;
            return false;
        }
    }
    public void Disconnect()
    {
        AppState.IsClientConnected = false;
        try
        {
            socket?.Shutdown(SocketShutdown.Both);
            socket?.Close();
            socket?.Dispose();
        }
        catch
        {

        }
        socket = null;
    }
    public async Task<bool> Reconnect()
    {
        AppState.IsClientConnected = false;
        if (!string.IsNullOrEmpty(clientIP) && clientPort > 0)
        {
            return await Connect(clientIP, clientPort);
        }
        return false;
    }
    private async Task SendCommand(string message)
    {
        if (!AppState.IsClientConnected) return;
        byte[] data = Encoding.UTF8.GetBytes(message);
        int bytesSent = await socket.SendAsync(new ArraySegment<byte>(data), SocketFlags.None);
    }
    private async Task TakeData()
    {
        byte[] buffer = new byte[1024];
        StringBuilder sb = new StringBuilder();
        while (AppState.IsClientConnected && socket.Connected)
        {
            try
            {
                int bytesGot = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                if (bytesGot > 0)
                {
                    string data = Encoding.UTF8.GetString(buffer, 0, bytesGot);
                    sb.Append(data);
                    string alldata = sb.ToString();
                    string[] lines = alldata.Split('\n');
                    // обработка всех полных строк, кроме последней
                    for (int i = 0; i < lines.Length - 1; i++)
                    {
                        ReadLineData(lines[i].Trim());
                    }
                    // сохранение неполной строки для следующей итерации
                    sb.Clear();
                    if (lines.Length > 0 && !string.IsNullOrEmpty(lines[lines.Length - 1]))
                    {
                        sb.Append(lines[lines.Length - 1]);
                    }

                    // Защита от бесконечного роста (на всякий случай)
                    if (sb.Length > 100)
                    {
                        Console.LogWarning($"Слишком длинный буфер: {sb}");
                        sb.Clear();
                    }
                }
                else break;
            }
            catch (Exception ex)
            {
                Console.Log($"Ошибка приема: {ex.Message}");
                break;
            }
        }
        if (sb.Length > 0)
        {
            ReadLineData(sb.ToString().Trim());
        }
        AppState.IsClientConnected = false;
        Console.Log("Соединение разорвано.");
    }
    private void ReadLineData(string line)
    {
        if (string.IsNullOrWhiteSpace(line)) {  return; }
        if (float.TryParse(line, NumberStyles.Any, CultureInfo.InvariantCulture, out float result))
        {
            // численные данные: буферизация и усреднение для подавления шума
            posBuffer.Enqueue(result);
            if (posBuffer.Count > bufferSize) { posBuffer.Dequeue(); }
            float sum = 0f;
            foreach (float value in posBuffer) { sum += value; }
            currentValue = sum / posBuffer.Count;
            OnDataFlow?.Invoke(currentValue); // передача данных суппорту
        }
        else
        {
            // текстовые данные: состояние шлюза, ошибки
            Console.Log($"[СЕРВЕР]: {line}");
            if (line.StartsWith("OK")) { AppState.comState = AppState.COMState.Connected; }
            if (line.StartsWith("ERROR")) { AppState.comState = AppState.COMState.Error; }
            if (line.StartsWith("READING:STARTED")) { AppState.comState = AppState.COMState.Reading; }
            if (line.StartsWith("READING:STOPPED")) { AppState.comState = AppState.COMState.Stoped; }
            if (line.StartsWith("COM:DISCONNECTED")) { AppState.comState = AppState.COMState.Disconnected; }
        }
    }
    private void OnDestroy()
    {
        COMPanel.OnCOM_SET -= HandleCommandSet;
        COMPanel.OnComConnect -= HandleConnect;
        COMPanel.OnComDisconnect -= HandleDisconnect;
        COMPanel.OnComStop -= HandleStop;
        AppState.IsClientConnected = false;
        socket?.Close();  
    }
    private async void HandleCommandSet(string command)
    {
        AppState.comState = AppState.COMState.Connecting;
        await SendCommand(command);
        AppState.comState = AppState.COMState.Connected;
    }
    private async void HandleConnect()
    {
        await SendCommand("START_READING");
        AppState.comState = AppState.COMState.Reading;
    }
    private async void HandleDisconnect()
    {
        await SendCommand("DISCONNECT_COM");
        AppState.comState = AppState.COMState.Disconnected;
    }
    private async void HandleStop()
    {
        await SendCommand("STOP_READING");
        AppState.comState = AppState.COMState.Stoped;
    }
    public async Task ExitCommandSet()
    {
        AppState.comState = AppState.COMState.Disconnected;
        await SendCommand("STOP_SERVER");
    }
};


