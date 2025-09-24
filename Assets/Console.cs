using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
public static class Console
{
    private static List<string> messages = new List<string>();
    private static int maxMessages = 50;
    private static bool initialized = false;

    public static System.Action<string> OnMessageAdded;
    public static System.Action OnConsoleCleared;
    public static void Initialize(int maxMessagesCount = 50)
    {
        if (initialized) return;

        maxMessages = maxMessagesCount;
        messages.Clear();
        initialized = true;

        Log("Консоль активирована");
    }
    public static void Log(string message)
    {
        if (!initialized) Initialize();

        string formattedMessage = $"[{System.DateTime.Now:HH:mm:ss}] {message}";
        messages.Add(formattedMessage);
        // ограничение размера буфера...
        if (messages.Count > maxMessages)
        {
            messages.RemoveAt(0);
        }

        OnMessageAdded?.Invoke(formattedMessage); // рассылка события для ConsoleUI
        Debug.Log(message);
    }
    public static void LogWarning(string message)
    {
        string warningMessage = $"<color=yellow> {message}</color>";
        Log(warningMessage);
    }
    public static void LogError(string message)
    {
        string errorMessage = $"<color=red> {message}</color>";
        Log(errorMessage);
    }
    public static void LogSuccess(string message)
    {
        string successMessage = $"<color=green> {message}</color>";
        Log(successMessage);
    }

    public static void Clear()
    {
        messages.Clear();
        OnConsoleCleared?.Invoke();
        Log("Консоль очищена");
    }

    public static string GetFullLog()
    {
        return string.Join("\n", messages);
    }
    public static List<string> GetMessages()
    {
        return new List<string>(messages);
    }
    public static int GetMessageCount()
    {
        return messages.Count;
    }
}

