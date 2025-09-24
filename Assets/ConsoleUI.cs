using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleUI : MonoBehaviour
{
    [Header("Настройки UI")]
    public GameObject consolePanel;
    public TMP_Text consoleText;
    public ScrollRect scrollRect;
    public RectTransform contentTransform;
    public RectTransform viewportTransform;
    public bool showOnStart = true;
    void Start()
    {
        Console.OnMessageAdded += AddMessageToUI;
        Console.OnConsoleCleared += ClearUI;
        ConsoleIM.OnConsoleTogglePressed += ToggleConsole;
        ConsoleIM.OnClearConsolePressed += ClearConsole;
        Console.Initialize();
        
        if (scrollRect == null) scrollRect = GetComponent<ScrollRect>();
        if (consoleText == null) consoleText = GetComponent<TMP_Text>();
        if ((consolePanel == null) && (scrollRect != null)) consolePanel = scrollRect.gameObject;
        if ((contentTransform == null) && (scrollRect != null)) contentTransform = scrollRect.content;
        if ((viewportTransform == null) && (scrollRect != null)) viewportTransform = scrollRect.viewport;
        if (consolePanel != null)
        {
            consolePanel.SetActive(showOnStart);
        }
    }
    void Update()
    {
        if (FocusManager.Instance.IsInputFieldFocused)
            return;
        ConsoleIM.Update();
    }
    private void AddMessageToUI(string message)
    {
        UpdateConsoleText();
    }
    private void ClearUI()
    {
        UpdateConsoleText();
    }
    private void UpdateConsoleText()
    {
        if (consoleText != null)
        {
            consoleText.text = Console.GetFullLog();
        }
        ForceUpdateText();
        StartCoroutine(DelayScrollCheck());
    }
    private void ScrollToBottom()
    {
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }
    private void ToggleConsole()
    {
        if (consolePanel != null)
        {
            consolePanel.SetActive(!consolePanel.activeSelf);
        }
    }
    private void OnDestroy()
    {
        Console.OnMessageAdded -= AddMessageToUI;
        Console.OnConsoleCleared -= ClearUI;
        ConsoleIM.OnClearConsolePressed -= ClearConsole;
        ConsoleIM.OnConsoleTogglePressed -= ToggleConsole;
    }
    public void ClearConsole()
    {
        Console.Clear();
    }
    public void ToggleConsoleVisibility()
    {
        ToggleConsole();
    }
    private IEnumerator DelayScrollCheck()
    {
        yield return null; // ожидание окончания кадра

        Canvas.ForceUpdateCanvases(); // принудительное обновление
        // логика сравнния высот content и viewport
        float contentHeight = contentTransform.rect.height;
        float viewportHeight = viewportTransform.rect.height;
        
        if (contentHeight > viewportHeight)
        {
            ScrollToBottom();
        }
    }
    private void ForceUpdateText()
    {
        consoleText.ForceMeshUpdate();
        Canvas.ForceUpdateCanvases();
        // ручной расчет высоты
        contentTransform.sizeDelta = new Vector2( 
            contentTransform.sizeDelta.x,
            consoleText.preferredHeight - 180f);
    }
}
