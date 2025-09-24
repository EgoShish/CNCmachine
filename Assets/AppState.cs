using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AppState
{
    public enum ServerState { Disconnected, Starting, Ready, Error }
    public static ServerState serverState = ServerState.Disconnected;

    // Клиент  
    public static bool IsClientConnected = false;

    // COM-порт
    public enum COMState { Disconnected, Connecting, Connected, Reading, Stoped, Error }
    public static COMState comState = COMState.Disconnected;

    // Режим суппорта
    public enum SupportMode { Position, Velocity }
    public static SupportMode supportMode = SupportMode.Velocity;
}
