using UnityEngine;
public class WakeUpEyeAnimator : MonoBehaviour
{
    [Header("Material & Properties")]
    public Material wakeUpMaterial;
    public string eyeOpenProperty = "_EyeOpen";
    public string aspectProperty = "_EllipseAspect"; // 新增

    [Header("Animation Settings")]
    public float wakeUpDuration = 3.0f;
    public AnimationCurve wakeUpCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Blink Shape Settings")]
    [Tooltip("闭眼时的椭圆纵横比（值越大，横向缝隙越窄）")]
    public float aspectWhenClosed = 100f;
    [Tooltip("睁眼时的椭圆纵横比（正常视野，通常1.0~2.0）")]
    public float aspectWhenOpen = 0f;

    private float timer;
    private bool isAnimating;

    void Start()
    {
        if (wakeUpMaterial == null)
        {
            Debug.LogError("WakeUpEyeAnimator: 未指定材质！");
            enabled = false;
            return;
        }
        StartWakeUp();
    }

    public void StartWakeUp()
    {
        timer = 0f;
        isAnimating = true;
        SetEyeOpen(0f);
        SetAspect(aspectWhenClosed); // 初始闭眼：横向细缝
    }

    void Update()
    {
        if (!isAnimating) return;

        timer += Time.deltaTime;
        float progress = Mathf.Clamp01(timer / wakeUpDuration);
        float curvedProgress = wakeUpCurve.Evaluate(progress);

        // 同时插值 EyeOpen 和 Aspect
        SetEyeOpen(curvedProgress);
        float currentAspect = Mathf.Lerp(aspectWhenClosed, aspectWhenOpen, curvedProgress);
        SetAspect(currentAspect);

        if (progress >= 1.0f)
        {
            FinishAnimation();
        }
    }

    private void SetEyeOpen(float value)
    {
        wakeUpMaterial.SetFloat(eyeOpenProperty, value);
    }

    private void SetAspect(float value)
    {
        wakeUpMaterial.SetFloat(aspectProperty, value);
    }

    private void FinishAnimation()
    {
        isAnimating = false;
        SetEyeOpen(1f);
        SetAspect(aspectWhenOpen);
        this.enabled = false;
        Debug.Log("睁眼动画播放完毕");
    }
}