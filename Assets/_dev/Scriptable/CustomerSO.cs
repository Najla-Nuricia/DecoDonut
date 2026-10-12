using UnityEngine;

[CreateAssetMenu(fileName = "NewCustomer", menuName = "DecoDonut/CustomerSO")]
public class CustomerSO : ScriptableObject
{
    [Header("Customer Info")]
    public string customerName; // Nama pelanggan
    public Sprite customerImage; // Gambar karakter pelanggan
    
    [Header("Order Reference")]
    public OrderSO customerOrder; // Referensi pesanan pelanggan
}