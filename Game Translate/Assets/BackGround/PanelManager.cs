using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    public static PanelManager Instance { get; private set; }

    public Button globalCloseButton;
    private IPanelController currentOpenPanel;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (globalCloseButton != null)
        {
            globalCloseButton.onClick.AddListener(OnGlobalCloseClick);
            globalCloseButton.gameObject.SetActive(false);
        }
    }

    public void RegisterOpenPanel(IPanelController panel)
    {
        currentOpenPanel = panel;
        if (globalCloseButton != null) globalCloseButton.gameObject.SetActive(true);
    }

    public void UnregisterOpenPanel()
    {
        currentOpenPanel = null;
        if (globalCloseButton != null) globalCloseButton.gameObject.SetActive(false);
    }

    private void OnGlobalCloseClick()
    {
        if (currentOpenPanel != null)
        {
            currentOpenPanel.Hide();
        }
        UnregisterOpenPanel();
    }
}