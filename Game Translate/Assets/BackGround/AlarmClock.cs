using Unity.Burst.CompilerServices;
using UnityEngine;
using static AC2;

public class AlarmClock : MonoBehaviour
{
    [Header("闹钟设置")]
    public int clockID;
    public AudioSource alarmSource;
    public GameObject targetPanel;        // 对应的 UI Panel
    public GameObject clothObject;        // 对应的桌布（拖入）
    public float minAngleToInteract = 30f; // 需要桌布掀开至少多少度才能点击

    private bool isRinging = false;
    private Drag clothFlip;

    public bool IsRinging => isRinging;

    private AudioClip originalClip;   // 保存原始铃声

    void Start()
    {
        if (alarmSource == null) alarmSource = GetComponent<AudioSource>();
        if (alarmSource != null) alarmSource.Stop();
        if (targetPanel != null) targetPanel.SetActive(false);
        if (clothObject != null) clothFlip = clothObject.GetComponent<Drag>();
        else Debug.LogWarning($"闹钟 {clockID} 没有关联桌布，将始终允许点击");
        if (alarmSource != null)
            originalClip = alarmSource.clip; // 记录初始铃声
    }

    void Update()
    {
        // 鼠标左键按下时进行 2D 射线检测
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (clothFlip != null && clothFlip.CurrentAngle < minAngleToInteract)
            {
                Debug.Log($"桌布角度不足，无法点击闹钟");
                return;
            }
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                // 检查桌布是否掀开足够角度
                bool canInteract = true;
                if (clothFlip != null)
                {
                    canInteract = clothFlip.CurrentAngle >= minAngleToInteract;
                    if (!canInteract)
                        Debug.Log($"闹钟 {clockID} 被桌布盖住（角度 {clothFlip.CurrentAngle:F1}°），无法点击");
                }
                if (canInteract)
                {
                    OnClockClicked();
                }
            }
        }
    }

    void OnClockClicked()
    {
        if (targetPanel == null)
        {
            Debug.LogError($"闹钟 {clockID}: targetPanel 为空");
            return;
        }

        // 通过接口获取控制器
        IPanelController panelCtrl = targetPanel.GetComponent<IPanelController>();
        if (panelCtrl == null)
        {
            Debug.LogError($"闹钟 {clockID}: targetPanel 上没有实现 IPanelController 的组件");
            return;
        }

        Debug.Log($"闹钟 {clockID} 调用 panelCtrl.Show(this)");
        panelCtrl.Show(this);
    }

    public void StartRinging()
    {
        if (isRinging) return;
        isRinging = true;
        if (alarmSource != null && !alarmSource.isPlaying)
            alarmSource.Play();
        Debug.Log($"闹钟 {clockID} 开始响铃");
    }

    public void StopRinging()
    {
        if (!isRinging)
        {
            Debug.Log($"闹钟 {clockID} 未在响铃，无需停止");
            return;
        }
        isRinging = false;
        if (alarmSource != null)
        {
            alarmSource.Stop();
            Debug.Log($"闹钟 {clockID} 音频已停止");
        }
        else
        {
            Debug.LogError($"闹钟 {clockID} 的 alarmSource 为空！");
        }
        // 关闭面板（如果还开着）
        if (targetPanel != null && targetPanel.activeSelf)
        {
            AlarmPanelController panelCtrl = targetPanel.GetComponent<AlarmPanelController>();
            if (panelCtrl != null) panelCtrl.Hide();
            else targetPanel.SetActive(false);
        }
    }
    // 在 AlarmClock 类中添加以下方法

    public void IncreaseVolume(float delta = 0.1f)
    {
        if (alarmSource != null)
            alarmSource.volume = Mathf.Min(1f, alarmSource.volume + delta);
    }

    public void SwitchToAlternateClip(AudioClip newClip)
    {
        if (alarmSource != null && newClip != null)
        {
            bool wasPlaying = alarmSource.isPlaying;
            alarmSource.clip = newClip;
            if (wasPlaying)
                alarmSource.Play();
        }
    }
    public void ToggleAlternateClip(AudioClip alternateClip)
    {
        if (alarmSource == null || alternateClip == null) return;

        bool wasPlaying = alarmSource.isPlaying;
        if (alarmSource.clip == originalClip)
            alarmSource.clip = alternateClip;
        else
            alarmSource.clip = originalClip;

        if (wasPlaying)
            alarmSource.Play();
    }
}