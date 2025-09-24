using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class COMPanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField comField;
    [SerializeField] private TMP_InputField baudField;
    [SerializeField] private Button connect;
    [SerializeField] private Button disconnect;
    [SerializeField] private Button reConnectServer;
    [SerializeField] private Button load;
    [SerializeField] private Button stop;

    private string[] command = new string[3];

    public static event Action<string> OnCOM_SET;
    public static event Action OnComConnect;
    public static event Action OnComDisconnect;
    public static event Action OnComStop;
    public static event System.Action OnReConnectServer;
    private void Start()
    {
        // настройка параметров по умолчанию
        command[0] = "SET_COM";

        comField.text = "COM3";
        baudField.text = "115200";
        command[1] = ":" + comField.text;
        command[2] = ":" + baudField.text; 
        comField.onEndEdit.AddListener(ComFieldSet);
        baudField.onEndEdit.AddListener(BaudFieldSet);
        
        connect.onClick.AddListener(ConnectCommand);
        disconnect.onClick.AddListener(DisconnectCommand);
        reConnectServer.onClick.AddListener(ReConnectServer);
        load.onClick.AddListener(SendCommand);
        stop.onClick.AddListener(StopCommand);
    }
    public void ComFieldSet(string comPort)
    {
        command[1] = ":" + comPort;
    }
    public void BaudFieldSet(string baudNum)
    {
        command[2] = ":" + baudNum;
    }
    private void SendCommand()
    {
        if ((AppState.comState == AppState.COMState.Disconnected) || (AppState.comState == AppState.COMState.Error))
        {
            string resultCommand = string.Join("", command); // сборка команды
            Console.Log($"Команда отправлена: {resultCommand}.");
            OnCOM_SET?.Invoke(resultCommand); // отправка через событие
        }
        else { return; }
    }
    public void ConnectCommand()
    {
        if ((AppState.comState == AppState.COMState.Connected) || (AppState.comState == AppState.COMState.Stoped))
        {
            OnComConnect?.Invoke();
        }
        else { return; }
    }
    public void DisconnectCommand()
    {
        if ((AppState.comState == AppState.COMState.Disconnected) || (AppState.comState == AppState.COMState.Connecting)) { return; }
        OnComDisconnect?.Invoke();
    }
    public void ReConnectServer()
    {
        AppState.comState = AppState.COMState.Disconnected;
        OnReConnectServer?.Invoke();
    }
    public void StopCommand()
    {
        if ((AppState.comState == AppState.COMState.Reading) || (AppState.comState == AppState.COMState.Connected))
        {
            OnComStop?.Invoke();
        }
        else { return; }
    }
}
