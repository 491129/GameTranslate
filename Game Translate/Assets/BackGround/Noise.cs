using UnityEngine;

public class Noise : MonoBehaviour
{
    public float maxAngle = 60f;
    public float sensitivity = 0.5f;   // 滑动灵敏度
    public bool invert = false;         // 是否反转方向

    private Material mat;
    private float currentAngle = 0f;
    private bool isSliding = false;
    private float lastMouseX;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        // 鼠标左键按下时开始滑动
        if (Input.GetMouseButtonDown(0))
        {
            isSliding = true;
            lastMouseX = Input.mousePosition.x;
            Debug.Log("开始滑动");
        }

        // 鼠标左键抬起时结束
        if (Input.GetMouseButtonUp(0))
        {
            isSliding = false;
            Debug.Log("结束滑动");
        }

        // 滑动中
        if (isSliding)
        {
            float deltaX = Input.mousePosition.x - lastMouseX;
            float deltaAngle = deltaX * sensitivity * (invert ? -1f : 1f);
            currentAngle += deltaAngle;
            // 限制角度范围 0 ~ maxAngle
            currentAngle = Mathf.Clamp(currentAngle, 0f, maxAngle);
            lastMouseX = Input.mousePosition.x;

            // 应用角度
            mat.SetFloat("_Angle", currentAngle);
            Debug.Log("当前角度: " + currentAngle);
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 30), "按下鼠标左键并左右滑动 -> 角度: " + Mathf.Round(currentAngle) + "°");
    }
}