using UnityEngine;

public class Tool : MonoBehaviour
{
    [Header("拖拽设置")]
    public bool freezeX = false;          // 锁定X轴移动
    public bool freezeY = false;          // 锁定Y轴移动

    [Header("玩家检测")]
    public Transform player;              // 拖入玩家的Transform
    public float interactionRange = 2f;   // 玩家需要靠近的距离（半径）
    public Vector2 detectionCenterOffset = Vector2.zero; // 检测中心偏移（世界坐标偏移，向右/向上为正）

    private bool isDragging = false;
    private Vector3 offset;
    private float originalZ;              // 记录枕头的原始Z轴

    void Start()
    {
        originalZ = transform.position.z;
    }

    void Update()
    {
        // 计算检测中心的世界坐标（2D，忽略Z）
        Vector2 detectionCenter = new Vector2(transform.position.x, transform.position.y) + detectionCenterOffset;
        bool playerNearby = false;
        if (player != null)
        {
            Vector2 playerPos = new Vector2(player.position.x, player.position.y);
            float dist = Vector2.Distance(playerPos, detectionCenter);
            playerNearby = dist <= interactionRange;
        }

        // 只有玩家靠近时才能拖拽，否则强制停止拖拽
        if (!playerNearby)
        {
            isDragging = false;
            return;
        }

        // 鼠标按下：2D射线检测是否点中枕头
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                isDragging = true;
                // 获取鼠标世界坐标时保持Z轴为originalZ，避免物体跑到相机位置
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, originalZ));
                offset = transform.position - mouseWorldPos;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, originalZ));
            Vector3 targetPos = mouseWorldPos + offset;
            if (freezeX) targetPos.x = transform.position.x;
            if (freezeY) targetPos.y = transform.position.y;
            targetPos.z = originalZ;   // 强制保持原始Z轴
            transform.position = targetPos;
        }
    }

    // 在Scene视图中显示交互范围（方便调试）
    void OnDrawGizmosSelected()
    {
        Vector2 detectionCenter = new Vector2(transform.position.x, transform.position.y) + detectionCenterOffset;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(new Vector3(detectionCenter.x, detectionCenter.y, transform.position.z), interactionRange);
    }
}