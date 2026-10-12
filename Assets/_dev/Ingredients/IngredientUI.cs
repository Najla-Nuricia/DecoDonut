
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class IngredientUI : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Ingredient Data")]
    public IngredientSO ingredientData;

    [Header("References")]
    [SerializeField] private Image ingredientImage;

    private Canvas canvas;
    private RectTransform dragVisual;

    public IngredientSO IngredientData => ingredientData;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();

        if (canvas != null)
            canvas = canvas.rootCanvas;
    }

    public void Setup(IngredientSO ingredient)
    {
        ingredientData = ingredient;

        if (ingredientImage != null && ingredient != null)
            ingredientImage.sprite = ingredient.ingredientImage;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (ingredientData == null || canvas == null)
            return;

        GameObject visual = new GameObject(
            "DraggedIngredient",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image)
        );

        dragVisual = visual.GetComponent<RectTransform>();
        dragVisual.SetParent(canvas.transform, false);

        Image image = visual.GetComponent<Image>();
        image.sprite = ingredientData.ingredientImage;
        image.preserveAspect = true;
        image.raycastTarget = false;

        RectTransform sourceRect = GetComponent<RectTransform>();
        dragVisual.sizeDelta = sourceRect.rect.size;
        dragVisual.localScale = Vector3.one;

        UpdateDragPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateDragPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragVisual != null)
            Destroy(dragVisual.gameObject);

        dragVisual = null;
    }

    private void UpdateDragPosition(PointerEventData eventData)
    {
        if (dragVisual == null)
            return;

        RectTransform canvasRect =
            canvas.transform as RectTransform;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint))
        {
            dragVisual.localPosition = localPoint;
        }
    }
}