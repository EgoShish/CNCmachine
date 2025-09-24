using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AppState
{
    public enum ServerState { Disconnected, Starting, Ready, Error }
    public static ServerState serverState = ServerState.Disconnected;

    // ������  
    public static bool IsClientConnected = false;

    // COM-����
    public enum COMState { Disconnected, Connecting, Connected, Reading, Stoped, Error }
    public static COMState comState = COMState.Disconnected;

    // ����� ��������
    public enum SupportMode { Position, Velocity }
    public static SupportMode supportMode = SupportMode.Velocity;
}
