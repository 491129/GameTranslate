using UnityEngine;

public class Drag : MonoBehaviour
{
    [Header("翻开设置")]
    public float maxAngle = 90f;
    public float sensitivity = 0.5f;
    public bool invert = false;

    [Header("闹钟")]
    public GameObject alarmClock;
    public float revealAngle = 30f;

    [Header("玩家检测")]
    public Transform player;
    public float interactionRange = 2f;
    public Vector2 detectionCenterOffset = Vector2.zero;

    private Material mat;
    private float currentAngle = 0f;
    private bool isDragging = false;
    private float lastMouseX;

    public float CurrentAngle => currentAngle;
    void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null) mat = rend.material;
        if (alarmClock != null) alarmClock.SetActive(false);
    }

    void Update()
    {
        // 计算检测中心（世界坐标，忽略Z轴）
        Vector2 detectionCenter = new Vector2(transform.position.x, transform.position.y) + detectionCenterOffset;
        bool playerNearby = false;
        if (player != null)
        {
            Vector2 playerPos = new Vector2(player.position.x, player.position.y);
            float dist = Vector2.Distance(playerPos, detectionCenter);
            playerNearby = dist <= interactionRange;
        }

        // 玩家不在范围内：自动合上桌布
        if (!playerNearby)
        {
            isDragging = false;  // 强制停止拖拽
            if (currentAngle > 0)
            {
                // 平滑减小角度
                currentAngle = Mathf.Lerp(currentAngle, 0f, Time.deltaTime * 5f);
                if (currentAngle < 0.01f) currentAngle = 0f;
                mat.SetFloat("_Angle", currentAngle);
                // 更新闹钟显隐
                if (alarmClock != null)
                    alarmClock.SetActive(currentAngle >= revealAngle);
            }
            return; // 不再处理输入
        }

        // 玩家在范围内：处理鼠标拖拽
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
            {
                isDragging = true;
                lastMouseX = Input.mousePosition.x;
            }
        }
        if (Input.GetMouseButtonUp(0)) isDragging = false;

        if (isDragging)
        {
            float deltaX = Input.mousePosition.x - lastMouseX;
            float deltaAngle = deltaX * sensitivity * (invert ? -1f : 1f);
            currentAngle += deltaAngle;
            currentAngle = Mathf.Clamp(currentAngle, 0f, maxAngle);
            lastMouseX = Input.mousePosition.x;
            mat.SetFloat("_Angle", currentAngle);
            if (alarmClock != null)
                alarmClock.SetActive(currentAngle >= revealAngle);
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector2 detectionCenter = new Vector2(transform.position.x, transform.position.y) + detectionCenterOffset;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(new Vector3(detectionCenter.x, detectionCenter.y, transform.position.z), interactionRange);
    }
}