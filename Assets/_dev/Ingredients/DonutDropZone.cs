
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DonutDropZone : MonoBehaviour, IDropHandler
{
    [Header("Drop Area")]
    [SerializeField] private RectTransform toppingContainer;
    [SerializeField] private GameObject toppingPrefab;

    [Header("Selected Ingredients")]
    [SerializeField]
    private List<IngredientSO> selectedIngredients = new();

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip dropSound;

    public IReadOnlyList<IngredientSO> SelectedIngredients
        => selectedIngredients;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject draggedObject = eventData.pointerDrag;

        if (draggedObject == null)
            return;

        IngredientUI ingredientUI =
            draggedObject.GetComponent<IngredientUI>();

        if (ingredientUI == null ||
            ingredientUI.IngredientData == null)
            return;

        IngredientSO ingredient = ingredientUI.IngredientData;

        // Buat gambar ingredient baru di meja
        GameObject item = Instantiate(
            toppingPrefab,
            toppingContainer
        );

        Image image = item.GetComponent<Image>();

        if (image != null)
        {
            image.sprite = ingredient.ingredientImage;
            image.preserveAspect = true;
            image.raycastTarget = false;
        }

        RectTransform itemRect =
            item.GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            toppingContainer,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPosition
        );

        itemRect.anchoredPosition = Vector2.zero;
        itemRect.localScale = Vector3.one;

        // Simpan ingredient untuk validasi resep
        selectedIngredients.Add(ingredient);

        if (dropSound != null)
            audioSource.PlayOneShot(dropSound);

        Debug.Log("Ingredient ditambahkan: " +
                  ingredient.ingredientName);
    }

    public void ClearIngredients()
    {
        selectedIngredients.Clear();

        foreach (Transform child in toppingContainer)
        {
            Destroy(child.gameObject);
        }
    }
}