using UnityEngine;
using UnityEngine.UI;

public class OverlayManager : MonoBehaviour
{
    private static OverlayManager instance;
    private GameObject overlay;
    private System.Action onOverlayClick;

    public static void Show(System.Action onClick)
    {
        if (instance == null)
        {
            GameObject go = new GameObject("OverlayManager");
            instance = go.AddComponent<OverlayManager>();
        }
        instance.ShowOverlay(onClick);
    }

    public static void Hide()
    {
        if (instance != null) instance.HideOverlay();
    }

    private void ShowOverlay(System.Action onClick)
    {
        if (overlay == null)
        {
            overlay = new GameObject("GlobalOverlay");
            overlay.transform.SetParent(GetComponentInParent<Canvas>()?.transform ?? FindObjectOfType<Canvas>().transform);
            overlay.transform.SetAsFirstSibling();
            Image img = overlay.AddComponent<Image>();
            img.color = new Color(0, 0, 0, 0);
            img.raycastTarget = true;
            RectTransform rect = overlay.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            Button btn = overlay.AddComponent<Button>();
            btn.onClick.AddListener(() => { onOverlayClick?.Invoke(); HideOverlay(); });
        }
        overlay.SetActive(true);
        onOverlayClick = onClick;
    }

    private void HideOverlay()
    {
        if (overlay != null) overlay.SetActive(false);
    }
}