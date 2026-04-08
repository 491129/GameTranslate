using UnityEngine;
using UnityEngine.UI;

public class AlarmPanelController : MonoBehaviour
{
    public Button closeButton;
    public Button exitButton;
    private AlarmClock currentClock;

    void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseClicked);
        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitClicked);
        gameObject.SetActive(false);
    }

    public void Show(AlarmClock clock)
    {
        currentClock = clock;
        gameObject.SetActive(true);
        if (closeButton != null)
            closeButton.interactable = (clock != null && clock.IsRinging);
        Debug.Log($"显示面板，关联闹钟 ID: {(clock != null ? clock.clockID.ToString() : "null")}");
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        currentClock = null;
        Debug.Log("面板关闭");
    }

    private void OnCloseClicked()
    {
        if (currentClock != null && currentClock.IsRinging)
        {
            currentClock.StopRinging();
            // 可在此处通知 AlarmManager 增加计数（后续添加）
        }
        Hide();
    }

    private void OnExitClicked()
    {
        Hide();
    }
}