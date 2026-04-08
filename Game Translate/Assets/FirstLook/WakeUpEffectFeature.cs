using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WakeUpEffectFeature : ScriptableRendererFeature
{
    [SerializeField] private Material material;
    private WakeUpEffectPass wakeUpPass;

    public override void Create()
    {
        wakeUpPass = new WakeUpEffectPass(material);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (material != null)
            renderer.EnqueuePass(wakeUpPass);
    }
}