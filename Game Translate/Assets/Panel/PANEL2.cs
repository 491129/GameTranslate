using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class PANEL2 : MonoBehaviour,  IPanelController
{
    [Header("按钮")]
    public Button volumeUpBtn;      // 增大音量
    public Button colorAnimBtn;     // 单击变色
    public Button toggleClipBtn;    // 切换音频（来回）
    public Button closeBtn;         // 关闭闹铃

    [Header("颜色效果")]
    public Image targetImage;       // 变色的目标图片（例如面板背景）
    public Color[] cycleColors = { Color.red, new Color(1f, 0.5f, 0f), Color.yellow, Color.white };
    public float cycleDuration = 1.25f; // 每个颜色持续1.25秒，4个颜色共5秒

    [Header("音频切换")]
    public AudioClip alternateClip; // 备用的铃声

    public AlarmClock currentClock;
   // private bool isOpen = false;
    private Tween colorTween;
    private Color originalColor;


    void Start()
    {
        if (targetImage != null)
        {
            originalColor = Color.white; // 或 new Color(0.8f, 0.6f, 0.4f) 等
            targetImage.color = originalColor;
            // 强制原始颜色不透明
            originalColor.a = 1f;
            targetImage.color = originalColor;
        }

        volumeUpBtn.onClick.AddListener(OnVolumeUp);
        colorAnimBtn.onClick.AddListener(OnColorAnimClick);
        toggleClipBtn.onClick.AddListener(OnToggleClip);
        closeBtn.onClick.AddListener(OnClose);

        gameObject.SetActive(false);
    }
    void OnColorAnimClick()
    {
        if (targetImage == null) return;
        // 停止当前动画
        if (colorTween != null && colorTween.IsActive()) colorTween.Kill();
        // 创建颜色循环序列
        Sequence seq = DOTween.Sequence();
        foreach (Color col in cycleColors)
        {
            seq.Append(targetImage.DOColor(col, cycleDuration));
        }
        // 动画结束后恢复原始颜色
        seq.OnComplete(() => {
            if (targetImage != null) targetImage.color = originalColor;
        });
        colorTween = seq.Play();
    }
    void Update()
    {
        // 按 F 键关闭面板（不停止闹钟）
        if (Input.GetKeyDown(KeyCode.F) && gameObject.activeSelf)
        {
            Hide();
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
        if (targetImage == null) return;
        // 停止之前的动画
        if (colorTween != null && colorTween.IsActive()) colorTween.Kill();
        // 创建颜色循环动画：依次变换到 cycleColors 中的颜色，每个颜色持续 cycleDuration 秒，无限循环
        Sequence seq = DOTween.Sequence();
        foreach (Color col in cycleColors)
        {
            seq.Append(targetImage.DOColor(col, cycleDuration));
        }
        colorTween = seq.SetLoops(-1); // 无限循环
    }


    void OnHoldEnd()
    {
        if (targetImage == null) return;
        if (colorTween != null && colorTween.IsActive()) colorTween.Kill();
        targetImage.color = originalColor;
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
        // 停止颜色动画并恢复原色
        if (colorTween != null && colorTween.IsActive()) colorTween.Kill();
        if (targetImage != null) targetImage.color = originalColor;
        if (currentClock != null && currentClock.IsRinging)
            currentClock.StopRinging();
    }

    public void Show(AlarmClock clock)
    {
        currentClock = clock;
        // 确保颜色恢复
        if (targetImage != null)
        {
            // 确保显示时不透明
            targetImage.color = originalColor;
        }
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (colorTween != null && colorTween.IsActive()) colorTween.Kill();
        if (targetImage != null) targetImage.color = originalColor;
        gameObject.SetActive(false);
        currentClock = null;
    }
}