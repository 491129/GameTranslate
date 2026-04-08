//using UnityEngine;
//using UnityEngine.UI;

//public class AlarmPanelController : MonoBehaviour
//{
//    public Button closeButton;
//    public Button exitButton;
//    private AlarmClock currentClock;

//    void Start()
//    {
//        if (closeButton != null)
//            closeButton.onClick.AddListener(OnCloseClicked);
//        if (exitButton != null)
//            exitButton.onClick.AddListener(OnExitClicked);
//        gameObject.SetActive(false);
//    }

//    //public void Show(AlarmClock clock)
//    //{
//    //    currentClock = clock;
//    //    gameObject.SetActive(true);
//    //    if (closeButton != null)
//    //        closeButton.interactable = (clock != null && clock.IsRinging);
//    //    Debug.Log($"显示面板，关联闹钟 ID: {(clock != null ? clock.clockID.ToString() : "null")}");
//    //}
//    public void Show(AlarmClock clock)
//    {
//        Debug.Log("Show 调用");
//        currentClock = clock;
//        gameObject.SetActive(true);
//        Debug.Log($"激活后 activeSelf = {gameObject.activeSelf}");
//    }
//    void TestKeepActive() { Debug.Log("5秒后仍激活: " + gameObject.activeSelf); }

//    public void Hide()
//    {
//        gameObject.SetActive(false);
//        currentClock = null;
//        Debug.Log("面板关闭");
//    }

//    private void OnCloseClicked()
//    {
//        if (currentClock != null && currentClock.IsRinging)
//        {
//            currentClock.StopRinging();
//            // 可在此处通知 AlarmManager 增加计数（后续添加）
//        }
//        Hide();
//    }

//    private void OnExitClicked()
//    {
//        Hide();
//    }
//}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AlarmPanelController : MonoBehaviour, IPointerClickHandler
{
    [Header("UI 组件")]
    public Slider slider;               // 滑块组件
    public GameObject clockImage;       // 中间的闹钟图片（用于点击不关闭）

    private AlarmClock currentClock;
    private bool hasClosed = false;     // 防止多次关闭

    void Start()
    {
        if (slider != null)
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        else
            Debug.LogError("AlarmPanelController: Slider 未赋值！");

        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        // 每次面板激活时重置滑块（不触发事件）
        if (slider != null)
        {
            slider.SetValueWithoutNotify(0);
            slider.value = 0;   // 强制刷新
        }
        hasClosed = false;
    }

    public void Show(AlarmClock clock)
    {
        Debug.Log("Show 调用");
        if (clock == null) return;
        currentClock = clock;
        hasClosed = false;
        if (slider != null) slider.SetValueWithoutNotify(0);

        // 确保所在 Canvas 激活
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null && !canvas.gameObject.activeSelf)
        {
            canvas.gameObject.SetActive(true);
            Debug.Log("Canvas 未激活，已激活");
        }

        // 强制刷新布局
        Canvas.ForceUpdateCanvases();
        CreateTestPanel();
        Transform t = transform;
        while (t != null)
        {
            if (!t.gameObject.activeSelf)
                t.gameObject.SetActive(true);
            t = t.parent;
        }

        // 再次强制激活自己
        gameObject.SetActive(true);

        // 强制刷新 Canvas
        Canvas.ForceUpdateCanvases();

        Debug.Log($"最终 activeSelf: {gameObject.activeSelf}");
    }
    public void CreateTestPanel()
    {
        // 创建一个测试面板
        GameObject testPanel = new GameObject("TestPanel");
        testPanel.transform.SetParent(GetComponentInParent<Canvas>().transform, false);
        RectTransform rect = testPanel.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(300, 200);
        rect.anchoredPosition = Vector2.zero;
        Image img = testPanel.AddComponent<Image>();
        img.color = Color.green;
        testPanel.AddComponent<Button>(); // 方便点击关闭
        testPanel.SetActive(true);
        Debug.Log("测试面板已创建");
    }
    public void Hide()
    {
        gameObject.SetActive(false);
        currentClock = null;
        Debug.Log("面板隐藏，清空 currentClock");
    }

    private void OnSliderValueChanged(float value)
    {
        if (currentClock == null) return;   // 避免面板关闭后继续执行
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
        // 获取被点击的物体
        GameObject clicked = eventData.pointerCurrentRaycast.gameObject;
        // 如果点击的是闹钟图片或其子物体，则不关闭面板
        if (clockImage != null && clicked != null)
        {
            if (clicked == clockImage || clicked.transform.IsChildOf(clockImage.transform))
                return;
        }
        // 否则关闭面板（闹钟继续响）
        Hide();
    }
}