
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ServeValidator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CustomerManager customerManager;
    [SerializeField] private DonutDropZone dropZone;

    [Header("Canvases")]
    [SerializeField] private GameObject kitchenCanvas;
    [SerializeField] private GameObject customerCanvas;


    [Header("Result Panels")]
    [SerializeField] private GameObject congratsPanel;
    [SerializeField] private GameObject youLosePanel;
    [SerializeField] private GameObject endDayPanel;

    [Header("Coin UI")]
    [SerializeField] private TMP_Text coinText;

    private int coins;

    public int Coins => coins;

    private void Start()
    {
        UpdateCoinUI();
    }

    public void ValidateOrder()
    {
        // Ambil customer yang sedang dilayani
        CustomerSO customer = customerManager.CurrentCustomer;

        if (customer == null || customer.customerOrder == null)
        {
            return;
        }

        // Ambil resep milik customer tersebut
        OrderSO order = customer.customerOrder;

        List<IngredientSO> selected =
            new List<IngredientSO>(dropZone.SelectedIngredients);

        List<IngredientSO> recipe =
            new List<IngredientSO>(order.recipeIngredients);

        bool isCorrect = selected.Count == recipe.Count;

        if (isCorrect)
        {
            foreach (IngredientSO ingredient in recipe)
            {
                int index = selected.IndexOf(ingredient);

                if (index == -1)
                {
                    isCorrect = false;
                    break;
                }

                selected.RemoveAt(index);
            }
        }

        // Berpindah dari dapur ke tampilan hasil customer
        kitchenCanvas.SetActive(false);
        customerCanvas.SetActive(true);

        congratsPanel.SetActive(isCorrect);
        youLosePanel.SetActive(!isCorrect);

        if (isCorrect)
        {
            coins += order.rewardCoins;
            UpdateCoinUI();
        }
    }

    
    public void ContinueToNextCustomer()
    {
        congratsPanel.SetActive(false);
        youLosePanel.SetActive(false);

        dropZone.ClearIngredients();

        customerManager.CompleteCurrentOrder();

        // Tidak ada customer tersisa
        if (customerManager.CurrentCustomer == null)
        {
            kitchenCanvas.SetActive(false);
            customerCanvas.SetActive(true);

            endDayPanel.SetActive(true);
            return;
        }

        // Masih ada customer berikutnya
        endDayPanel.SetActive(false);
        kitchenCanvas.SetActive(true);
        customerCanvas.SetActive(true);
    }


    public void RetryOrder()
    {
        // Tutup panel hasil kalah
        youLosePanel.SetActive(false);

        // Bersihkan ingredient dari meja
        dropZone.ClearIngredients();

        // Kembali ke dapur tanpa mengganti customer
        customerCanvas.SetActive(false);
        kitchenCanvas.SetActive(true);
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = coins.ToString();
        }
    }
}