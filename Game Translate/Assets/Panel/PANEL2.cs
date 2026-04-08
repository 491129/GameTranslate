using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PANEL2 : MonoBehaviour,  IPanelController
{
    [Header("按钮")]
    public Button volumeUpBtn;      // 增大音量
    public Button holdColorBtn;     // 按住变色
    public Button toggleClipBtn;    // 切换音频（来回）
    public Button closeBtn;         // 关闭闹铃

    [Header("颜色效果")]
    public Image targetImage;       // 变色的目标图片（例如面板背景）
    public Color holdColor = Color.red;

    [Header("音频切换")]
    public AudioClip alternateClip; // 备用的铃声

    public AlarmClock currentClock;
    private bool isOpen = false;

    void Start()
    {
        volumeUpBtn.onClick.AddListener(OnVolumeUp);
        toggleClipBtn.onClick.AddListener(OnToggleClip);
        closeBtn.onClick.AddListener(OnClose);

        // 为 holdColorBtn 添加指针按下/抬起事件
        AddPointerEvent(holdColorBtn, EventTriggerType.PointerDown, (data) => OnHoldStart());
        AddPointerEvent(holdColorBtn, EventTriggerType.PointerUp, (data) => OnHoldEnd());
        AddPointerEvent(holdColorBtn, EventTriggerType.PointerExit, (data) => OnHoldEnd());

        gameObject.SetActive(false);
    }
    void Update()
    {
        if (!isOpen) return;

        // 检测鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            // 获取鼠标位置
            Vector2 mousePos = Input.mousePosition;
            // 检查鼠标位置是否在面板的 RectTransform 内
            if (!RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), mousePos))
            {
                // 点击在面板外部，关闭面板
                Hide();
            }
        }
    }

    void AddPointerEvent(Button btn, EventTriggerType eventType, UnityEngine.Events.UnityAction<BaseEventData> callback)
    {
        EventTrigger trigger = btn.GetComponent<EventTrigger>();
        if (trigger == null) trigger = btn.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventType;
        entry.callback.AddListener(callback);
        trigger.triggers.Add(entry);
    }

    void OnHoldStart()
    {
        if (targetImage != null)
            targetImage.color = holdColor;
    }

    void OnHoldEnd()
    {
        if (targetImage != null)
            targetImage.color = Color.white; // 恢复白色
    }

    void OnVolumeUp()
    {
        if (currentClock != null)
            currentClock.IncreaseVolume(0.2f);
    }

    void OnToggleClip()
    {
        if (currentClock != null && alternateClip != null)
            currentClock.ToggleAlternateClip(alternateClip);
    }

    void OnClose()
    {
        if (currentClock != null)
        {
            // 强制停止 AudioSource
            AudioSource source = currentClock.GetComponent<AudioSource>();
            if (source != null) source.Stop();
            // 同步标志（可选）
            currentClock.StopRinging();
        }
        Hide();
    }

    public void Show(AlarmClock clock)
    {
        currentClock = clock;
        gameObject.SetActive(true);
        isOpen = true;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        isOpen = false;
        currentClock = null;
    }

    // 点击外部关闭面板（闹铃继续响）
    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    GameObject clicked = eventData.pointerCurrentRaycast.gameObject;
    //    if (clicked != null)
    //    {
    //        // 如果点击的是任何按钮（或其子物体），则不关闭面板
    //        if (clicked == volumeUpBtn.gameObject || clicked.transform.IsChildOf(volumeUpBtn.transform) ||
    //            clicked == holdColorBtn.gameObject || clicked.transform.IsChildOf(holdColorBtn.transform) ||
    //            clicked == toggleClipBtn.gameObject || clicked.transform.IsChildOf(toggleClipBtn.transform) ||
    //            clicked == closeBtn.gameObject || clicked.transform.IsChildOf(closeBtn.transform))
    //            return;
    //    }
    //    Hide(); // 仅关闭面板，不停止闹铃
    //}
}