using UnityEngine;

public class Walk : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;          // 移动速度

    private Rigidbody2D rb;               // 刚体组件
    private SpriteRenderer spriteRenderer; // 图片渲染器（用于翻转）
    private Animator animator;            // 动画控制器

    private Vector2 moveInput;            // 存储移动输入

    void Awake()
    {
        // 获取组件
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // 如果没有找到 SpriteRenderer，则尝试从子物体获取（可选）
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        // 获取 WASD 输入（水平 A/D，垂直 W/S）
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(horizontal, vertical).normalized;

        // 控制朝向（仅根据水平输入翻转图片）
        if (horizontal != 0)
        {
            // 方法1：使用 SpriteRenderer.flipX 翻转图片（推荐）
            spriteRenderer.flipX = (horizontal < 0);

            // 方法2：使用 Transform.localScale 翻转（二选一，注释掉其中一个）
            // transform.localScale = new Vector3(horizontal > 0 ? 1 : -1, 1, 1);
        }

        // 更新动画参数（假设有一个 "Speed" 浮点参数）
        if (animator != null)
        {
            float speed = moveInput.magnitude;   // 是否有移动
            animator.SetFloat("Speed", speed);
        }
    }

    void FixedUpdate()
    {
        rb.velocity = moveInput * moveSpeed;
    }
}