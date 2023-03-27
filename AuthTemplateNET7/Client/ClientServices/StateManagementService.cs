using AuthTemplateNET7.Shared.PublicModels;
using AuthTemplateNET7.Shared.SharedServices;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace AuthTemplateNET7.Client.ClientServices;

public class StateManagementService
{
    bool initialized = false;
    private readonly IJSRuntime js;
    const string SHOPPING_CART_KEY = "SHOPPING_CART";

    public event Action CartChanged;

    public Order Order { get; set; }

    public int ItemsCount { get; set; }

    public StateManagementService(IJSRuntime js)
    {
        this.js = js;
    }

    public async Task AddToCartAsync(Product product)
    {
        OrderItem item = new OrderItem(product);
        Order.OrderItems.Add(item);
        Order.Total += item.LineTotal;
        await SaveCartToLocalStorageAsync(Order);
        ItemsCount++;
        CartChanged?.Invoke();
    }

    public async Task ClearCartAsync()
    {
        Order = new();
        ItemsCount = 0;

        while(CartChanged == null) await Task.Delay(10);

        CartChanged.Invoke();

        await js.InvokeVoidAsync("tf.removeItem", SHOPPING_CART_KEY);
    }

    public async Task Init()
    {
        if(initialized) return;
        initialized = true;

        var json = await js.InvokeAsync<string>("tf.getItem", SHOPPING_CART_KEY);

        if (json != null)
        {
            Order = json.FromJson<Order>();
            ItemsCount = Order.OrderItems.Count;
        }
        else Order = new();
        CartChanged?.Invoke();
    }

    public async Task RemoveFromCartAsync(OrderItem orderItem)
    {
        Order.OrderItems.Remove(orderItem);
        Order.Total -= orderItem.LineTotal;
        await SaveCartToLocalStorageAsync(Order);
        ItemsCount--;
        CartChanged.Invoke();
    }

    public async Task SaveCartToLocalStorageAsync(Order order)
    {
        await js.InvokeVoidAsync("tf.setItem", SHOPPING_CART_KEY, Order.ToJson());
    }
}
