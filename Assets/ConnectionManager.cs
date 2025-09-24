using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ConnectionManager : MonoBehaviour
{
    [SerializeField] private ServerManager serverManager;
    [SerializeField] private Client client;
    public static System.Action OnServerReady;


    private void Start()
    {
        SB_MyPuTTy.OnServerStarted += InitializeConnection;
        COMPanel.OnReConnectServer += ReConnectionServer;
        ExitMenu.exitActive += NormalShutdown;
    }

    private async void InitializeConnection()
    {
        // запуск сервера
        bool serverStarted = await serverManager.StartServer();
        if (!serverStarted) { return; }

        // подключение клиента к серверу
        bool clientConnected = await client.Connect();
        if (!clientConnected) { return; }

        // уведомление UI о готовности системы
        OnServerReady?.Invoke();
    }

    private async void ReConnectionServer()
    {
        await client.ExitCommandSet();
        await Task.Delay(1000);

        await serverManager.RestartServer();
        await client.Connect();
    }
    
    private async void NormalShutdown()
    {
        await client.ExitCommandSet();

        await Task.Delay(1000);

        client.Disconnect();

        await serverManager.StopServer();
    }

    private async Task ForceShutdown()
    {
        client.Disconnect();

        await serverManager.StopServer();
    }

    private async void OnApplicationQuit()
    {
        await ForceShutdown();
    }

    private void OnDestroy()
    {
        SB_MyPuTTy.OnServerStarted -= InitializeConnection;
        COMPanel.OnReConnectServer -= ReConnectionServer;
        ExitMenu.exitActive -= NormalShutdown;
    }
}
