using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class IngredientDragEffects : MonoBehaviour,
    IBeginDragHandler, IEndDragHandler
{
    [System.Serializable]
    public class IngredientItem
    {
        public RectTransform item;
        public AudioClip dragSound;
        public AudioClip dropSound;
    }

    [Header("Ingredients")]
    [SerializeField] private IngredientItem[] ingredients;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    [Header("Animation")]
    [SerializeField] private float dragScale = 1.15f;
    [SerializeField] private float rotationAngle = 8f;
    [SerializeField] private float duration = 0.15f;

    private RectTransform currentItem;
    private Vector3 originalScale;
    private Quaternion originalRotation;
    private AudioClip currentDragSound;
    private AudioClip currentDropSound;

    public void OnBeginDrag(PointerEventData eventData)
    {
        currentItem = eventData.pointerDrag.GetComponent<RectTransform>();

        foreach (var ingredient in ingredients)
        {
            if (ingredient.item == currentItem)
            {
                currentDragSound = ingredient.dragSound;
                currentDropSound = ingredient.dropSound;
                break;
            }
        }

        if (currentItem == null) return;

        originalScale = currentItem.localScale;
        originalRotation = currentItem.localRotation;

        currentItem.DOKill();

        if (audioSource != null && currentDragSound != null)
            audioSource.PlayOneShot(currentDragSound);

        currentItem.DOScale(originalScale * dragScale, duration)
            .SetEase(Ease.OutBack);

        currentItem.DOLocalRotate(
            new Vector3(0, 0, rotationAngle), duration
        ).SetEase(Ease.OutQuad);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentItem == null) return;

        currentItem.DOKill();

        if (audioSource != null && currentDropSound != null)
            audioSource.PlayOneShot(currentDropSound);

        currentItem.DOScale(originalScale, duration)
            .SetEase(Ease.OutBack);

        currentItem.DOLocalRotateQuaternion(originalRotation, duration)
            .SetEase(Ease.OutBack);

        currentItem = null;
    }
}