using UnityEngine;
using DG.Tweening;
using System.Collections;

public class BubbleUI : MonoBehaviour
{
    [SerializeField] private Transform[] bubbles;
    [SerializeField] private float duration = 0.5f;

    public void ShowBubbles()
    {
        StopAllCoroutines();
        StartCoroutine(AnimateBubbles());
    }

    private IEnumerator AnimateBubbles()
    {
        yield return null;

        foreach (Transform bubble in bubbles)
        {
            if (bubble == null) continue;

            Vector3 originalScale = bubble.localScale;

            bubble.DOKill();
            bubble.localScale = Vector3.zero;

            bubble.DOScale(originalScale, duration)
                .SetEase(Ease.OutBack);
        }
    }
}