using UnityEngine;

public class IngredientManager : MonoBehaviour
{
    [Header("Ingredient UI")]
    [SerializeField] private IngredientUI[] ingredientItems;

    private void Start()
    {
        SetupIngredients();
    }

    public void SetupIngredients()
    {
        foreach (IngredientUI item in ingredientItems)
        {
            if (item == null)
                continue;

            if (item.ingredientData == null)
            {
                Debug.LogWarning(
                    "IngredientSO belum diatur pada " + item.name
                );
                continue;
            }

            item.Setup(item.ingredientData);
        }
    }
}