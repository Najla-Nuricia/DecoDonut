using UnityEngine;

[CreateAssetMenu(fileName = "NewIngredient", menuName = "DecoDonut/IngredientSO")]
public class IngredientSO : ScriptableObject
{
    [Header("Ingredient Info")]
    public string ingredientName; // Nama bahan
    public Sprite ingredientImage; // Gambar bahan
}