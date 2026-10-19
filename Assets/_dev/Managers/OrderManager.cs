using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text orderNameUI;
    [SerializeField] private Transform recipeContainer;
    [SerializeField] private Image recipePrefab;

    public void ShowOrder(OrderSO order)
    {
        ClearOrder();

        if (order == null)
        {
            Debug.LogWarning("OrderSO tidak ditemukan!");
            return;
        }

        orderNameUI.text = order.orderName;

        if (order.recipeIngredients == null)
            return;

        foreach (IngredientSO ingredient in order.recipeIngredients)
        {
            if (ingredient == null || ingredient.ingredientImage == null)
                continue;

            Image image = Instantiate(recipePrefab, recipeContainer);
            image.sprite = ingredient.ingredientImage;
            image.enabled = true;
        }
    }

    public void ClearOrder()
    {
        orderNameUI.text = "";

        foreach (Transform child in recipeContainer)
        {
            Destroy(child.gameObject);
        }
    }
}