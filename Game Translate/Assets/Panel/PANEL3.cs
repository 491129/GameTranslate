using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening; 

public class PANEL3 : MonoBehaviour, IPanelController
{
    [Header("指针容器（空物体，位于表盘中心）")]
    public Transform hourHandContainer;   // 时针的父物体（旋转中心）
    public Transform minuteHandContainer; // 分针的父物体（旋转中心）

    [Header("关闭闹钟目标")]
    public int targetHour = 7;        // 目标时针位置（0-11，0=12点）
    public int targetMinute = 8;      // 目标分针位置（0-11，8=40分）

    private AlarmClock currentClock;
    private int currentHourIndex = 0;
    private int currentMinuteIndex = 0;
    private bool isDraggingHour = false;
    private bool isDraggingMinute = false;
    //private GameObject overlay;
    private bool isClosing = false;

    void Start()
    {
        SetHourIndex(0);
        SetMinuteIndex(0);
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && gameObject.activeSelf)
        {
            Hide();
            return;
        }

        if (isClosing) return;
        // 检测拖拽
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;
            // 检测是否点击到时针容器（或其子物体）
            if (IsPointerOverUIElement(hourHandContainer.gameObject, mousePos))
                isDraggingHour = true;
            else if (IsPointerOverUIElement(minuteHandContainer.gameObject, mousePos))
                isDraggingMinute = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDraggingHour = false;
            isDraggingMinute = false;
        }

        if (isDraggingHour)
        {
            float angle = GetAngleFromMouse(hourHandContainer.position);
            int index = Mathf.RoundToInt(angle / 30f) % 12;
            SetHourIndex(index);
        }
        else if (isDraggingMinute)
        {
            float angle = GetAngleFromMouse(minuteHandContainer.position);
            int index = Mathf.RoundToInt(angle / 30f) % 12;
            SetMinuteIndex(index);
        }

        // 检查是否达到目标时间
        if (!isClosing && currentHourIndex == targetHour && currentMinuteIndex == targetMinute)
        {
            if (currentClock != null && currentClock.IsRinging)
            {
                StartCoroutine(CloseWithFeedback());
            }
        }
    }
    private System.Collections.IEnumerator CloseWithFeedback()
    {
        isClosing = true;
        // 播放面板弹跳动画（拉伸）
        transform.DOScale(new Vector3(1.2f, 1.2f, 1f), 0.2f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.2f);
        transform.DOScale(Vector3.one, 0.1f);
        yield return new WaitForSeconds(0.7f); // 总等待2秒（动画0.3秒+1.7秒）

        // 停止闹钟并关闭面板
        if (currentClock != null && currentClock.IsRinging)
            currentClock.StopRinging();
        Hide();
        isClosing = false;
    }

    // 检测鼠标是否在指定 GameObject 的 RectTransform 范围内（包括子物体）
    private bool IsPointerOverUIElement(GameObject target, Vector2 screenPos)
    {
        GraphicRaycaster raycaster = GetComponentInParent<GraphicRaycaster>();
        if (raycaster == null) return false;
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = screenPos;
        var results = new System.Collections.Generic.List<RaycastResult>();
        raycaster.Raycast(eventData, results);
        foreach (var result in results)
        {
            if (result.gameObject == target || result.gameObject.transform.IsChildOf(target.transform))
                return true;
        }
        return false;
    }
    private float GetAngleFromMouse(Vector2 centerWorldPos)
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 centerScreen = RectTransformUtility.WorldToScreenPoint(null, centerWorldPos);
        Vector2 delta = mousePos - centerScreen;
        float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        angle = (angle + 360) % 360;
        angle = 90 - angle; // 调整使0度向上
        if (angle < 0) angle += 360;
        return angle;
    }

    private void SetHourIndex(int index)
    {
        currentHourIndex = index;
        float angle = index * 30f; // 每小时30度
        hourHandContainer.localEulerAngles = new Vector3(0, 0, -angle);
    }

    private void SetMinuteIndex(int index)
    {
        currentMinuteIndex = index;
        float angle = index * 30f; // 每5分钟30度
        minuteHandContainer.localEulerAngles = new Vector3(0, 0, -angle);
    }


    public void Show(AlarmClock clock)
    {
        currentClock = clock;
        isClosing = false;
        transform.localScale = Vector3.one; // 重置缩放
        // 重置指针到 12:00（索引0）
        SetHourIndex(0);
        SetMinuteIndex(0);

        gameObject.SetActive(true);
        transform.SetAsLastSibling();

    }

    public void Hide()
    {
        // 停止任何正在播放的动画
        DOTween.Kill(transform);
        gameObject.SetActive(false);
        currentClock = null;
        isClosing = false;
        transform.localScale = Vector3.one; // 确保下次打开缩放正常
    }
}