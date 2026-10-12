using TMPro;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text orderNameUI;
    [SerializeField] private TMP_Text recipeUI;

    public void ShowOrder(OrderSO order)
    {
        if (order == null)
        {
            ClearOrder();
            Debug.LogWarning("OrderSO tidak ditemukan!");
            return;
        }

        orderNameUI.text = order.orderName;
        recipeUI.text = "Recipe:\n";

        if (order.recipeIngredients == null ||
            order.recipeIngredients.Count == 0)
        {
            recipeUI.text += "No ingredients";
            return;
        }

        foreach (IngredientSO ingredient in order.recipeIngredients)
        {
            if (ingredient != null)
            {
                recipeUI.text += "- "
                    + ingredient.ingredientName + "\n";
            }
        }
    }

    public void ClearOrder()
    {
        orderNameUI.text = "";
        recipeUI.text = "";
    }
}