using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private CustomerManager customerManager;
    [SerializeField] private OrderManager orderManager;

    private void Start()
    {
        StartDay();
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
    }

    public void CompleteOrder()
    {
        customerManager.CompleteCurrentOrder();
        UpdateCurrentOrder();
    }
}