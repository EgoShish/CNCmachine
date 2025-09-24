using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SupportMode : MonoBehaviour
{
    public static System.Action OnSupportModeChange;
    private Button modeChange;
    private void Start()
    {
        modeChange = GetComponent<Button>();
        modeChange.onClick.AddListener(OnSBClick);
    }
    private void OnSBClick()
    {
        OnSupportModeChange?.Invoke();
    }
    private void OnDestroy()
    {
        modeChange.onClick.RemoveListener(OnSBClick);
    }
}
