using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class AlarmManager : MonoBehaviour
{
    public static AlarmManager Instance { get; private set; }

    public List<AlarmClock> allClocks;          // 四个闹钟
    public float[] intervals = { 15f, 12f, 9f, 6f, 3f }; // 触发间隔（秒）
    public float initialDelay = 3f;              // 游戏开始后等待3秒再触发第一个闹钟

    [Header("玩家头痛动画")]
    public SpriteRenderer playerSprite;
    public Color[] headacheColors = { Color.red, new Color(1f, 0.5f, 0f), Color.yellow, Color.white };
    public float headacheColorDuration = 1.25f;
    public float headacheJumpHeight = 0.3f;
    public float headacheJumpDuration = 0.5f;

    private int intervalIndex = 0;
    private float gameStartTime;      // 游戏开始的时间戳
    private bool isGameOver = false;
    private bool isCountdownActive = false;  // 倒计时是否激活
    private float countdownTimer = 0f;

    private Tween playerColorTween;
    private Tween playerJumpTween;
    private Color originalPlayerColor;
    static public float ST;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    void Start()
    {
        if (allClocks == null || allClocks.Count == 0)
        {
            Debug.LogError("请将闹钟拖入 AlarmManager 的 allClocks 列表");
            return;
        }
        if (playerSprite != null)
            originalPlayerColor = playerSprite.color;
        gameStartTime = Time.time;
        StartCoroutine(DelayedStart());
    }

    void Update()
    {
        // 倒计时逻辑放在 Update 中，避免协程被意外停止
        if (isCountdownActive)
        {
            countdownTimer -= Time.deltaTime;
            if (countdownTimer <= 0f)
            {
                // 倒计时结束
                isCountdownActive = false;
                isGameOver = true;
                StopPlayerHeadache();
                float survivedTime = Time.time - gameStartTime;
                ST = survivedTime;
                Debug.Log($"游戏结束！坚持时间: {survivedTime:F2} 秒");
                // 停止所有闹钟声音
                foreach (var clock in allClocks)
                {
                    clock.StopRinging();
                }
                SceneManager.LoadScene("End");
            }
        }
    }
    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(initialDelay);
        TriggerRandomAlarm();
        StartCoroutine(TriggerLoop());
    }
    IEnumerator TriggerLoop()
    {
        while (!isGameOver)
        {
            // 获取本次需要等待的时间
            float waitTime;
            if (intervalIndex < intervals.Length)
            {
                waitTime = intervals[intervalIndex];
                intervalIndex++;
            }
            else
            {
                // 所有间隔用完后，一直使用最后一个间隔（3秒）
                waitTime = intervals[intervals.Length - 1];
            }

            yield return new WaitForSeconds(waitTime);

            if (isCountdownActive)
                continue; 

            // 选择一个未在响铃的闹钟（已经停止的闹钟可以再次被触发）
            var available = allClocks.Where(c => !c.IsRinging).ToList();
            if (available.Count == 0)
            {
                Debug.Log("所有闹钟都在响铃中，开始3秒倒计时...");
                    StartCountdown();
                continue;
            }
         
            int randomIndex = Random.Range(0, available.Count);
            AlarmClock selected = available[randomIndex];
            selected.StartRinging();
            Debug.Log($"触发闹钟 ID: {selected.clockID}, 间隔序号: {intervalIndex - 1}, 等待时间: {waitTime}s");
        }
    }


    void TriggerRandomAlarm()
    {
        // 过滤掉 null 元素
        var available = allClocks.Where(c => c != null && !c.IsRinging).ToList();
        if (available.Count == 0)
        {
            Debug.LogWarning("没有可触发的闹钟（可能全部为 null 或都在响铃）");
            return;
        }
        int rand = Random.Range(0, available.Count);
        available[rand].StartRinging();
    }
    void StartCountdown()
    {
        if (isCountdownActive) return; // 防止重复启动
        isCountdownActive = true;
        countdownTimer = 3f;
        StartPlayerHeadache();
        CloseAllPanels();
    }
    void CloseAllPanels()
    {
        foreach (var clock in allClocks)
        {
            if (clock.targetPanel != null)
            {
                // 尝试通过 IPanelController 关闭
                IPanelController panelCtrl = clock.targetPanel.GetComponent<IPanelController>();
                if (panelCtrl != null)
                    panelCtrl.Hide();
                else
                    clock.targetPanel.SetActive(false); // 后备方案
            }
        }
        Debug.Log("所有面板已强制关闭");
    }
    void StartPlayerHeadache()
    {
        if (playerSprite == null) return;
        // 颜色循环
        Sequence colorSeq = DOTween.Sequence();
        foreach (Color col in headacheColors)
        {
            colorSeq.Append(playerSprite.DOColor(col, headacheColorDuration));
        }
        playerColorTween = colorSeq.SetLoops(-1);
        // 弹跳拉伸（垂直方向）
        playerJumpTween = playerSprite.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0f), headacheJumpDuration, 1, 0).SetLoops(-1);
    }

    void StopPlayerHeadache()
    {
        if (playerColorTween != null && playerColorTween.IsActive())
            playerColorTween.Kill();
        if (playerJumpTween != null && playerJumpTween.IsActive())
            playerJumpTween.Kill();
        if (playerSprite != null)
        {
            playerSprite.color = originalPlayerColor;
            playerSprite.transform.localScale = Vector3.one;
        }
    }

    public bool CanStopAlarm()
    {
        return !isCountdownActive && !isGameOver;
    }

    void OnDestroy()
    {
        StopPlayerHeadache();
    }

}