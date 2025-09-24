using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ConsoleIM
{
    public static System.Action OnConsoleTogglePressed;
    public static System.Action OnClearConsolePressed;

    private static KeyCode toggleKey = KeyCode.F1;
    private static KeyCode clearKey = KeyCode.C;

    public static void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            OnConsoleTogglePressed?.Invoke();
        }
        if (Input.GetKeyDown(clearKey))
        { 
            OnClearConsolePressed?.Invoke();
        }
    }
}
