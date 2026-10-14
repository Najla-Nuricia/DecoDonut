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

    [Header("Panel Animations")]
    [SerializeField] private PanelAnimation congratsAnimation;
    [SerializeField] private PanelAnimation youLoseAnimation;
    [SerializeField] private PanelAnimation endDayAnimation;

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
        CustomerSO customer = customerManager.CurrentCustomer;

        if (customer == null || customer.customerOrder == null)
            return;

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

        kitchenCanvas.SetActive(false);
        customerCanvas.SetActive(true);

        if (isCorrect)
        {
            youLosePanel.SetActive(false);

            if (congratsAnimation != null)
                congratsAnimation.PopUp();
            else
                congratsPanel.SetActive(true);

            coins += order.rewardCoins;
            UpdateCoinUI();
        }
        else
        {
            congratsPanel.SetActive(false);

            if (youLoseAnimation != null)
                youLoseAnimation.PopUp();
            else
                youLosePanel.SetActive(true);
        }
    }

    public void ContinueToNextCustomer()
    {
        congratsPanel.SetActive(false);
        youLosePanel.SetActive(false);

        dropZone.ClearIngredients();
        customerManager.CompleteCurrentOrder();

        if (customerManager.CurrentCustomer == null)
        {
            kitchenCanvas.SetActive(false);
            customerCanvas.SetActive(true);

            if (endDayAnimation != null)
                endDayAnimation.PopUp();
            else
                endDayPanel.SetActive(true);

            return;
        }

        endDayPanel.SetActive(false);

        kitchenCanvas.SetActive(true);
        customerCanvas.SetActive(true);
    }

    public void RetryOrder()
    {
        youLosePanel.SetActive(false);
        dropZone.ClearIngredients();

        customerCanvas.SetActive(false);
        kitchenCanvas.SetActive(true);
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
            coinText.text = coins.ToString();
    }
}

