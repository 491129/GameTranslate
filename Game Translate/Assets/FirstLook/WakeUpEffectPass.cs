using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class WakeUpEffectPass : ScriptableRenderPass
{
    private Material material;
    private RenderTargetIdentifier source;
    private RenderTargetHandle tempTexture;

    public WakeUpEffectPass(Material material)
    {
        this.material = material;
        renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        tempTexture.Init("_TempWakeUpTexture");
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (material == null) return;

        CommandBuffer cmd = CommandBufferPool.Get("WakeUpEffectPass");
        RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
        opaqueDesc.depthBufferBits = 0;

        source = renderingData.cameraData.renderer.cameraColorTarget;

        cmd.GetTemporaryRT(tempTexture.id, opaqueDesc, FilterMode.Bilinear);

        // 应用材质效果
        Blit(cmd, source, tempTexture.Identifier(), material);
        Blit(cmd, tempTexture.Identifier(), source);

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void FrameCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(tempTexture.id);
    }
}