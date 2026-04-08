using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PANEL1 : MonoBehaviour, IPointerClickHandler
{
    public Slider slider;
    public GameObject clockImage;

    public AlarmClock currentClock;
    private bool hasClosed = false;

    void Start()
    {
        if (slider != null)
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        else
            Debug.LogError("Slider 未赋值！");

        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        // 每次面板激活时强制重置滑块（不触发事件）
        if (slider != null)
        {
            slider.SetValueWithoutNotify(0);
            // 强制刷新滑块的 handle 位置
            slider.value = 0;
        }
        hasClosed = false;
    }

    public void Show(AlarmClock clock)
    {
        if (clock == null)
        {
            Debug.LogError("Show 传入的 clock 为 null");
            return;
        }
        currentClock = clock;
        hasClosed = false;
        // 重置滑块
        if (slider != null)
        {
            slider.SetValueWithoutNotify(0);
            slider.value = 0;
        }
        gameObject.SetActive(true);
        Debug.Log($"面板显示，关联闹钟 {currentClock.clockID}");
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        currentClock = null;
        Debug.Log("面板隐藏，清空 currentClock");
    }



    private void OnSliderValueChanged(float value)
    {
        if (currentClock == null) return;  // 关键：避免在面板关闭后继续执行
        if (hasClosed) return;
        if (slider != null && value >= slider.maxValue)
        {
            hasClosed = true;
            if (currentClock.IsRinging)
                currentClock.StopRinging();
            Hide();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject clicked = eventData.pointerCurrentRaycast.gameObject;
        if (clockImage != null && clicked != null)
        {
            if (clicked == clockImage || clicked.transform.IsChildOf(clockImage.transform))
                return;
        }
        Hide();
    }
}