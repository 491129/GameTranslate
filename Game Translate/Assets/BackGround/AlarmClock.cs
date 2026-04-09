using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using static AC2;

public class AlarmClock : MonoBehaviour
{
    [Header("闹钟设置")]
    public int clockID;
    public AudioSource alarmSource;
    public GameObject targetPanel;        // 对应的 UI Panel
    [Header("桌布检测（可选）")]
    public GameObject clothObject;        // 对应的桌布（拖入）
    public float minAngleToInteract = 30f;
    //public float minAngleToInteract = 30f; // 需要桌布掀开至少多少度才能点击

    [Header("玩家交互距离")]
    public float interactionRange = 2f;   // 玩家必须靠近此距离才能点击

    private bool isRinging = false;
    public Transform player;
    private Drag clothFlip;

    public bool IsRinging => isRinging;

    private AudioClip originalClip;   // 保存原始铃声

    public UnityEvent onClockStopped;   // 闹钟停止时触发

    void Start()
    {
        if (alarmSource == null) alarmSource = GetComponent<AudioSource>();
        if (alarmSource != null) alarmSource.Stop();
        if (targetPanel != null) targetPanel.SetActive(false);
        //GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
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
           
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                    OnClockClicked();
            }
        }

    }

    void OnClockClicked()
    {
        if (player != null)
        {
            float dist = Vector2.Distance(transform.position, player.position);
            if (dist > interactionRange)
            {
                Debug.Log($"闹钟 {clockID} 太远（距离 {dist:F1}），无法操作");
                return;
            }
        }
        else
            Debug.Log("Player=null");
        if (clothFlip != null && clothFlip.CurrentAngle < minAngleToInteract)
        {
            Debug.Log($"闹钟 {clockID} 被桌布盖住（角度 {clothFlip.CurrentAngle:F1}°），无法点击");
            return;
        }

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
        if (!isRinging) return;

        // 检查是否允许关闭
        if (AlarmManager.Instance != null && !AlarmManager.Instance.CanStopAlarm())
        {
            Debug.Log($"闹钟 {clockID} 倒计时中或游戏已结束，无法关闭！");
            return;
        }

        isRinging = false;
        if (alarmSource != null) alarmSource.Stop();
        Debug.Log($"闹钟 {clockID} 停止响铃");
        onClockStopped?.Invoke();
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
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}