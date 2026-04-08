using UnityEngine;

public class FullscreenEffect : MonoBehaviour
{
    public Material fullscreenMaterial;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Debug.Log("OnRenderImage called"); // ÃÌº”’‚––
        if (fullscreenMaterial != null)
            Graphics.Blit(source, destination, fullscreenMaterial);
        else
            Graphics.Blit(source, destination);
    }
}