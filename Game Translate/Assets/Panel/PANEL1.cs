using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PANEL1 : MonoBehaviour, IPanelController
{
    public Slider slider;
    public GameObject clockImage;

    private AlarmClock currentClock;
    private bool hasClosed = false;

    void Start()
    {
        if (slider != null)
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        else
            Debug.LogError("Slider 未赋值！");

        gameObject.SetActive(false);
    }
    void Update()
    {
        // 按 F 键关闭面板（不停止闹钟）
        if (Input.GetKeyDown(KeyCode.F) && gameObject.activeSelf)
        {
            Hide();
        }
    }
    void OnEnable()
    {
        if (slider != null)
        {
            slider.SetValueWithoutNotify(0);
            slider.value = 0;
        }
        hasClosed = false;
    }

    public void Show(AlarmClock clock)
    {
        currentClock = clock;
        hasClosed = false;
        gameObject.SetActive(true);
        // 注册到全局管理器
       PanelManager.Instance.RegisterOpenPanel(this);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        PanelManager.Instance.UnregisterOpenPanel();
        currentClock = null;
    }

    private void OnSliderValueChanged(float value)
    {
        if (currentClock == null) return;
        if (hasClosed) return;
        if (slider != null && value >= slider.maxValue)
        {
            hasClosed = true;
            if (currentClock.IsRinging)
                currentClock.StopRinging();
            Hide();
        }
    }

}