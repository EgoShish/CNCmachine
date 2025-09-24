using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitMenu : MonoBehaviour
{
    public GameObject exitPanel;
    public Button exitButton;
    public static System.Action exitActive;
    private void Start()
    {
        exitPanel.SetActive(false);

        exitButton.onClick.AddListener(QuitGame);
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            ToggleMenue();
        }
    }
    private void ToggleMenue()
    {
        bool isActive = !exitPanel.activeSelf;
        exitPanel.SetActive(isActive);
    }
    void QuitGame()
    {
        exitActive?.Invoke();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
