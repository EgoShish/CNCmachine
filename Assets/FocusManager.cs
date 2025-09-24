using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FocusManager : MonoBehaviour
{
    public static FocusManager Instance { get; private set; }
    public static event System.Action<bool> OnFocusChanged;
    public bool IsInputFieldFocused { get; private set; }

    void Awake()
    {
        Initialize();
    }

    void Update()
    {
        CheckInputFocus();
    }

    private void Initialize()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("FocusManager initialized");
        }
        else
        {
            Debug.LogWarning("Duplicate FocusManager destroyed");
            Destroy(gameObject);
        }
    }

    private void CheckInputFocus()
    {
        bool newFocusState = CheckIfAnyInputFieldFocused();

        if (newFocusState != IsInputFieldFocused)
        {
            IsInputFieldFocused = newFocusState;
            OnFocusChanged?.Invoke(newFocusState);
        }
    }

    private bool CheckIfAnyInputFieldFocused()
    {
        if (EventSystem.current == null || EventSystem.current.currentSelectedGameObject == null)
            return false;

        GameObject selected = EventSystem.current.currentSelectedGameObject;
        return selected.GetComponent<InputField>() != null ||
               selected.GetComponent<TMPro.TMP_InputField>() != null;
    }
}

