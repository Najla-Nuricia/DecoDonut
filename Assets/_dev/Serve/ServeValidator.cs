using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ServeValidator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CustomerManager customerManager;
    [SerializeField] private DonutDropZone dropZone;
    [SerializeField] private CanvasTransition canvasTransition;

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
    [SerializeField] private float resultDelay = 0.75f;

    [Header("Coin UI")]
    [SerializeField] private TMP_Text coinText;

    private int coins;
    private bool isShowingResult;
    public int Coins => coins;

    private void Start()
    {
        UpdateCoinUI();
    }

    public void ValidateOrder()
    {
        if (isShowingResult)
            return;

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

        // Slide dari Kitchen ke Customer
        canvasTransition.ShowCustomer();

        isShowingResult = true;
        StartCoroutine(ShowResultAfterDelay(isCorrect, order.rewardCoins));
    }

    public void ContinueToNextCustomer()
    {
        isShowingResult = false;

        HideResultPanels();

        dropZone.ClearIngredients();
        customerManager.CompleteCurrentOrder();

        if (customerManager.CurrentCustomer == null)
        {
            ShowEndDayPanel();
            return;
        }

        GameManager gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager == null)
        {
            Debug.LogError(
                "ServeValidator: GameManager tidak ditemukan untuk memperbarui order."
            );
            return;
        }

        gameManager.UpdateCurrentOrder();

        if (endDayPanel != null)
            endDayPanel.SetActive(false);

        // Customer tetap tampil untuk customer berikutnya.
        // Tombol menuju dapur memanggil ShowKitchen().
    }

    public void RetryOrder()
    {
        youLosePanel.SetActive(false);
        dropZone.ClearIngredients();

        // Slide dari Customer kembali ke Kitchen
        canvasTransition.ShowKitchen();
    }

    public void GoToKitchen()
    {
        canvasTransition.ShowKitchen();
    }

    public void GoToCustomer()
    {
        canvasTransition.ShowCustomer();
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
        coinText.text = $"Here's your {coins} coins!";
    }

    private void HideResultPanels()
    {
        if (congratsAnimation != null)
            congratsAnimation.PopDown();
        else if (congratsPanel != null)
            congratsPanel.SetActive(false);

        if (youLoseAnimation != null)
            youLoseAnimation.PopDown();
        else if (youLosePanel != null)
            youLosePanel.SetActive(false);
    }

    private IEnumerator ShowResultAfterDelay(bool isCorrect, int rewardCoins)
    {
        yield return new WaitForSeconds(Mathf.Max(0f, resultDelay));

        if (isCorrect)
        {
            if (youLosePanel != null)
                youLosePanel.SetActive(false);

            if (congratsAnimation != null)
                congratsAnimation.PopUp();
            else if (congratsPanel != null)
                congratsPanel.SetActive(true);

            coins += rewardCoins;
            UpdateCoinUI();
        }
        else
        {
            if (congratsPanel != null)
                congratsPanel.SetActive(false);

            if (youLoseAnimation != null)
                youLoseAnimation.PopUp();
            else if (youLosePanel != null)
                youLosePanel.SetActive(true);
        }
    }

    private void ShowEndDayPanel()
    {
        if (endDayPanel == null && endDayAnimation == null)
        {
            Debug.LogError(
                "ServeValidator: Isi End Day Panel atau End Day Animation di Inspector."
            );
            return;
        }

        if (endDayPanel != null)
        {
            Transform parent = endDayPanel.transform.parent;
            while (parent != null)
            {
                parent.gameObject.SetActive(true);
                parent = parent.parent;
            }

            endDayPanel.SetActive(true);
        }

        if (endDayAnimation != null)
            endDayAnimation.PopUp();
    }
}
