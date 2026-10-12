using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewOrder", menuName = "DecoDonut/OrderSO")]
public class OrderSO : ScriptableObject
{
    [Header("Order Info")]
    public string orderName; // Nama pesanan
    public Sprite donutImage; // Gambar donat yang diinginkan
    
    [Header("Recipe & Reward")]
    public List<IngredientSO> recipeIngredients; // Daftar bahan resep
    public int rewardCoins; // Jumlah koin yang diberikan
}