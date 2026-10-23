
using UnityEngine;
using DG.Tweening;

public class PanelAnimation : MonoBehaviour
{
    [SerializeField] private Transform popup;
    [SerializeField] private float duration = 0.4f;

    private Vector3 originalScale;
    private bool hasOriginalScale;

    private void Awake()
    {
        if (popup == null)
            popup = transform;

        CaptureOriginalScale();
    }

    public void PopUp()
    {
        CaptureOriginalScale();

        popup.DOKill();
        popup.gameObject.SetActive(true);
        popup.localScale = Vector3.zero;

        popup.DOScale(originalScale, duration)
            .SetEase(Ease.OutBack);
    }

    public void PopDown()
    {
        CaptureOriginalScale();

        popup.DOKill();

        popup.DOScale(Vector3.zero, duration)
            .SetEase(Ease.InBack)
            .OnComplete(() => popup.gameObject.SetActive(false));
    }

    public void HideImmediate()
    {
        CaptureOriginalScale();

        popup.DOKill();
        popup.localScale = originalScale;
        popup.gameObject.SetActive(false);
    }

    private void CaptureOriginalScale()
    {
        if (hasOriginalScale)
            return;

        if (popup == null)
            popup = transform;

        originalScale = popup.localScale;
        hasOriginalScale = true;
    }
}
