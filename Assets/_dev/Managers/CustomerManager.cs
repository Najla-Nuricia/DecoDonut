using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomerManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image customerImageUI;
    [SerializeField] private TMP_Text customerNameUI;
    [SerializeField] private Image orderBubbleUI;

    [Header("Daily Customers")]
    [SerializeField] private CustomerSO[] dailyCustomers;

    private int currentCustomerIndex = 0;

    public CustomerSO CurrentCustomer { get; private set; }


    public void StartDay()
    {
        currentCustomerIndex = 0;
        ShowNextCustomer();
    }

    public void ShowNextCustomer()
    {
        if (currentCustomerIndex >= dailyCustomers.Length)
        {
            EndDay();
            return;
        }

        CustomerSO nextCustomer = dailyCustomers[currentCustomerIndex];
        currentCustomerIndex++;

        ShowCustomer(nextCustomer);
    }

    private void ShowCustomer(CustomerSO newCustomer)
    {
        if (newCustomer == null)
        {
            Debug.LogWarning("Data pelanggan kosong!");
            ClearCustomerDisplay();
            return;
        }

        CurrentCustomer = newCustomer;

        // Menampilkan gambar pelanggan
        customerImageUI.sprite = newCustomer.customerImage;
        customerImageUI.gameObject.SetActive(true);

        // Menampilkan nama pelanggan
        customerNameUI.text = newCustomer.customerName;
        customerNameUI.gameObject.SetActive(true);

        // Menampilkan gambar pesanan donat
        if (newCustomer.customerOrder != null)
        {
            orderBubbleUI.sprite = newCustomer.customerOrder.donutImage;
            orderBubbleUI.gameObject.SetActive(true);
        }
        else
        {
            orderBubbleUI.gameObject.SetActive(false);
            Debug.LogWarning("Pelanggan tidak memiliki pesanan!");
        }
    }

    public void CompleteCurrentOrder()
    {
        ClearCustomerDisplay();
        ShowNextCustomer();
    }

    private void EndDay()
    {
        ClearCustomerDisplay();
        Debug.Log("Semua pelanggan hari ini sudah dilayani!");

        // TODO: Tampilkan UI hasil hari.
    }

    private void ClearCustomerDisplay()
    {
        CurrentCustomer = null;

        customerImageUI.gameObject.SetActive(false);
        customerNameUI.gameObject.SetActive(false);
        orderBubbleUI.gameObject.SetActive(false);
    }
}