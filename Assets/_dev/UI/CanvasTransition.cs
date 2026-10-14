using UnityEngine;
using DG.Tweening;
using System.Collections;

public class CanvasTransition : MonoBehaviour
{
[Header("Panels")]
[SerializeField] private RectTransform kitchenPanel;
[SerializeField] private RectTransform customerPanel;


[Header("Settings")]
[SerializeField] private float duration = 0.35f;
[SerializeField] private float slideDistance;

private bool isTransitioning;

private void Start()
{
    if (kitchenPanel == null || customerPanel == null)
    {
        Debug.LogError("CanvasTransition: Isi Kitchen Panel dan Customer Panel di Inspector.");
        return;
    }

    if (kitchenPanel == customerPanel)
    {
        Debug.LogError("CanvasTransition: Kitchen Panel dan Customer Panel tidak boleh object yang sama.");
        return;
    }

    kitchenPanel.gameObject.SetActive(false);
    customerPanel.gameObject.SetActive(true);
}

public void ShowKitchen()
{
    if (!isTransitioning)
        StartCoroutine(Transition(customerPanel, kitchenPanel, 1));
}

public void ShowCustomer()
{
    if (!isTransitioning)
        StartCoroutine(Transition(kitchenPanel, customerPanel, -1));
}

private IEnumerator Transition(
    RectTransform current,
    RectTransform next,
    int direction)
{
    if (current == null || next == null)
    {
        Debug.LogError(
            "CanvasTransition: KitchenPanel atau CustomerPanel belum diisi!"
        );
        yield break;
    }

    if (current == next)
    {
        Debug.LogError("CanvasTransition: Panel sumber dan tujuan tidak boleh sama.");
        yield break;
    }

    if (!gameObject.activeInHierarchy)
    {
        Debug.LogError(
            "CanvasTransition: Pasang script ini di object parent yang tetap aktif, bukan di salah satu panel."
        );
        yield break;
    }

    isTransitioning = true;

    Transform parent = current.parent;

    if (parent == null)
    {
        Debug.LogError(
            "CanvasTransition: Panel tidak punya parent."
        );
        isTransitioning = false;
        yield break;
    }

    if (next.parent != parent)
    {
        Debug.LogError(
            "KitchenPanel dan CustomerPanel harus punya parent yang sama!"
        );
        isTransitioning = false;
        yield break;
    }

    Vector2 currentOriginal = current.anchoredPosition;
    Vector2 nextOriginal = next.anchoredPosition;

    Canvas.ForceUpdateCanvases();

    RectTransform parentRect = parent as RectTransform;
    float width = parentRect != null ? parentRect.rect.width : 0f;

    width = Mathf.Max(width, current.rect.width, next.rect.width);

    Canvas canvas = current.GetComponentInParent<Canvas>();
    if (canvas != null && canvas.rootCanvas != null)
    {
        float canvasWidth = canvas.rootCanvas.pixelRect.width /
            canvas.rootCanvas.scaleFactor;
        width = Mathf.Max(width, canvasWidth);
    }

    if (slideDistance > 0f)
        width = slideDistance;

    if (width <= 0f)
    {
        Debug.LogError(
            "CanvasTransition: Jarak slide tidak bisa dihitung. Isi Slide Distance secara manual di Inspector."
        );
        isTransitioning = false;
        yield break;
    }

    next.gameObject.SetActive(true);

    current.DOKill();
    next.DOKill();

    next.anchoredPosition =
        nextOriginal + Vector2.right * width * direction;

    current.DOAnchorPos(
        currentOriginal - Vector2.right * width * direction,
        duration
    ).SetEase(Ease.InOutCubic);

    next.DOAnchorPos(nextOriginal, duration)
        .SetEase(Ease.InOutCubic);

    yield return new WaitForSeconds(duration);

    current.gameObject.SetActive(false);
    current.anchoredPosition = currentOriginal;
    next.anchoredPosition = nextOriginal;
    next.gameObject.SetActive(true);

    isTransitioning = false;
}


}
