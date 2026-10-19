using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private CustomerManager customerManager;
    [SerializeField] private OrderManager orderManager;

    [Header("Canvas")]
    [SerializeField] private GameObject customerCanvas;
    [SerializeField] private GameObject kitchenCanvas;

    [Header("Bubble Animation")]
    [SerializeField] private BubbleUI customerBubble;
    [SerializeField] private BubbleUI kitchenBubble;

    private void Start()
    {
        StartDay();
        ShowCustomerCanvas();
    }

    public void ShowCustomerCanvas()
    {
        customerCanvas.SetActive(true);
        kitchenCanvas.SetActive(false);
    }

    public void ShowKitchenCanvas()
    {
        customerCanvas.SetActive(false);
        kitchenCanvas.SetActive(true);

        if (kitchenBubble != null)
            kitchenBubble.ShowBubbles();
    }

    public void StartDay()
    {
        customerManager.StartDay();
        UpdateCurrentOrder();
    }

    public void UpdateCurrentOrder()
    {
        CustomerSO customer = customerManager.CurrentCustomer;

        if (customer == null)
        {
            orderManager.ClearOrder();
            return;
        }

        orderManager.ShowOrder(customer.customerOrder);

        if (customerBubble != null)
            customerBubble.ShowBubbles();
    }

    public void CompleteOrder()
    {
        customerManager.CompleteCurrentOrder();
        UpdateCurrentOrder();
    }
}
