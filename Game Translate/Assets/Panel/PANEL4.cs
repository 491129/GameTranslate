using UnityEngine;
using UnityEngine.UI;

public class PANEL4 : MonoBehaviour, IPanelController
{
    [Header("字母顺序")]
    public string targetWord = "WAKEUP";   // 六个字母
    public Image[] letterImages;           // 按顺序拖入六个 Image

    [Header("完成颜色")]
    public Color completedColor = Color.gray;

    public AlarmClock currentClock;
    private int currentIndex = 0;
    private bool isActive = false;

    // 引用玩家移动脚本（根据你的实际脚本名称修改）
    private Walk playerMovementScript;

    void Start()
    {
        gameObject.SetActive(false);
        // 查找玩家身上的移动脚本（例如 PlayerMovement 或 TopDownMovement）
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerMovementScript = player.GetComponent<Walk>(); // 替换为实际组件类型
    }

    void Update()
    {
        if (!isActive) return;

        // 获取当前帧输入的字符
        string input = Input.inputString;
        if (input.Length > 0)
        {
            char c = input[0];
            // 忽略控制字符（如退格、回车等）
            if (c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z')
            {
                CheckLetter(c);
            }
        }
            // 按 F 键关闭面板（不停止闹钟）
            if (Input.GetKeyDown(KeyCode.F) && gameObject.activeSelf)
            {
                Hide();
            }
    }

    void CheckLetter(char pressed)
    {
        if (currentIndex >= targetWord.Length) return;

        char required = targetWord[currentIndex];
        // 忽略大小写比较
        if (char.ToUpper(pressed) == char.ToUpper(required))
        {
            // 正确
            if (currentIndex < letterImages.Length)
                letterImages[currentIndex].color = completedColor;
            currentIndex++;

            // 全部完成
            if (currentIndex >= targetWord.Length)
            {
                OnAllLettersTyped();
            }
        }
        else
        {
            // 错误输入：可给予短暂反馈（如闪烁红色），但不重置进度
            Debug.Log($"错误输入: {pressed}，需要 {required}");
            // 可选：闪烁红色
            StartCoroutine(FlashRed(letterImages[currentIndex]));
        }
    }

    System.Collections.IEnumerator FlashRed(Image img)
    {
        Color original = img.color;
        img.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        img.color = original;
    }

    void OnAllLettersTyped()
    {
        Debug.Log("全部字母正确，关闭闹钟");
        if (currentClock != null && currentClock.IsRinging)
            currentClock.StopRinging();
        Hide();
    }

    // 禁用/启用玩家移动
    void SetPlayerMovementEnabled(bool enabled)
    {
        if (playerMovementScript != null)
            playerMovementScript.enabled = enabled;
    }

    // 实现 IPanelController
    public void Show(AlarmClock clock)
    {
        currentClock = clock;
        currentIndex = 0;
        isActive = true;

        // 重置所有字母颜色为白色
        foreach (var img in letterImages)
            img.color = Color.white;

        gameObject.SetActive(true);
        SetPlayerMovementEnabled(false);  // 禁止玩家移动
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        isActive = false;
        SetPlayerMovementEnabled(true);   // 恢复玩家移动
        currentClock = null;
    }
}