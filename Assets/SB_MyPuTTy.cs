using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SB_MyPuTTy : MonoBehaviour
{
    public static System.Action OnServerStarted;
    private Button SB_PuTTy;
    public GameObject comPanel;
    private void Start()
    {
        SB_PuTTy = GetComponent<Button>();
        SB_PuTTy.onClick.AddListener(OnSBDown);
        ConnectionManager.OnServerReady += PanelVisability;
    }
    public void OnSBDown()
    {
        if (AppState.serverState == AppState.ServerState.Starting) { return; }
        OnServerStarted?.Invoke();
    }
    private void PanelVisability()
    {
        if (comPanel != null)
        {
            comPanel.SetActive(!comPanel.activeSelf);
        }
    }
    private void OnDestroy()
    {
        SB_PuTTy.onClick.RemoveListener(OnSBDown);
        ConnectionManager.OnServerReady -= PanelVisability;
    }

}
