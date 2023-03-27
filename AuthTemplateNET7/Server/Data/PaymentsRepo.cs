using AuthTemplateNET7.Shared;
using AuthTemplateNET7.Shared.PublicModels;
using AuthTemplateNET7.Shared.PublicModels.SiteSettingModels.Models.DevSettings;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace AuthTemplateNET7.Server.Data;

public class PaymentsRepo : RepoBase
{
    public PaymentsRepo(DataContext dataContext) : base(dataContext) { }

    public async Task<bool> CreateOrUpdateOrderAsync(Order order)
    {
        if(order.Id == Guid.Empty) dataContext.Add(order);
        else dataContext.Update(order);

        var rows = await dataContext.TrySaveAsync($"Could not create or update order:\r\n{order.ToJson(true)}");

        if (rows < 1) return false;

        return true;
    }

    public async Task MaybeLogStripeWebhookActivityAsync(BafGlobals bafGlobals, string eventType, string json, bool saveChanges)
    {
        if (bafGlobals.LogStripeActivityStatus == LogStripeActivityStatus.NoLog) return;

        if(bafGlobals.LogStripeActivityStatus == LogStripeActivityStatus.NotSet)
        {
            await setStripeLoggingStatusAsync(bafGlobals);
        }

        if (bafGlobals.LogStripeActivityStatus == LogStripeActivityStatus.DoLog)
        {
            LogItem logItem = new($"Stripe webhook hit {eventType}") { StackTraceOrJson = json };
            dataContext.Add(logItem);
            if (saveChanges) await dataContext.TrySaveAsync();
        }
    }

#if DEBUG
    public async Task UpdateOrderAsync(string orderIdStr, OrderStatus orderStatus)
    {
        Guid.TryParse(orderIdStr, out Guid orderId);

        var order = await dataContext.Orders.Where(m => m.Id == orderId).FirstOrDefaultAsync();

        if(order == null)
        {
            LogItem logItem = new($"Could not find order with Id {orderIdStr}", bootstrapColor: BootstrapColor.Danger);
            await CreateLogItemAsync(logItem);
            return;
        }

        order.OrderStatus = orderStatus;
        dataContext.Update(order);

        await dataContext.TrySaveAsync($"Could not set OrderStatus to {orderStatus.ToString()} on Order.Id {orderIdStr}");
    }
#else
    public Task UpdateOrderAsync(string orderIdStr, OrderStatus orderStatus)
    {
        //docs https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-7.0/whatsnew#basic-executeupdate-examples

        Guid.TryParse(orderIdStr, out Guid orderId);

        return dataContext.Orders.Where(m => m.Id == orderId).ExecuteUpdateAsync(m => m.SetProperty(p => p.OrderStatus, orderStatus));
    }
#endif



    public async Task VerifyOrderTotalAsync(Order order)
    {
        if (order == null) throw new ArgumentNullException(nameof(order) + " CAN NOT BE NULL");

        if (order.OrderItems.Count == 0) throw new Exception("AN ORDER MUST HAVE AT LEAST ONE ORDERITEM");

        var orderItems = order.OrderItems;

        bool createLogitem = false;

        decimal orderTotal = 0;

        Dictionary<int, OrderItem> orderItemsDict = orderItems.ToDictionary(k => k.ProductId, v => v);

#if DEBUG
        var productsAnon = await dataContext.Products.AsNoTracking().Select(m =>
        new
        {
            m.Id,
            m.Price
        }).ToArrayAsync();

        foreach (var p in productsAnon)
        {
            if (orderItemsDict.ContainsKey(p.Id))
            {
                var curr = orderItemsDict[p.Id];

                if (curr.UnitPrice != p.Price)
                {
                    curr.UnitPrice = p.Price;
                    createLogitem = true;
                }

                orderTotal += curr.LineTotal;
            }
        }

#else
        //todo AFTER SQL verify this works as expected
        string sql = generateSql(orderItems);

        var products = await dataContext.Database.SqlQueryRaw<Product>(sql).AsNoTracking().ToArrayAsync();

        foreach ( var p in products )
        {
            var curr = orderItemsDict[p.Id];

            if (curr.UnitPrice != p.Price)
            {
                curr.UnitPrice = p.Price;
                createLogitem = true;
            }
            orderTotal += curr.LineTotal;
        }
#endif
        if (order.Total != orderTotal)
        {
            createLogitem = true;
            order.Total = orderTotal;
        }

        if(createLogitem)
        {
            LogItem item = new($"UnitPrices in Order are not the same as Product.Price(s): \r\n{order.ToJson(true)}", bootstrapColor: Shared.BootstrapColor.Warning);
            await dataContext.TrySaveAsync();
        }


    }

    string generateSql(List<OrderItem> items)
    {
        var productIds = items.Select(m => m.ProductId).ToArray();

        StringBuilder sb = new StringBuilder();

        sb.Append($"SELECT {nameof(Product.Id)}, {nameof(Product.Price)} FROM dbo.Products WHERE Id = {productIds[0]}");

        if (productIds.Length > 1)
        {
            for (int i = 1; i < productIds.Length; i++)
            {
                sb.Append($" OR Id = {productIds[i]}");
            }
        }

        return sb.ToString();
    }

    async Task setStripeLoggingStatusAsync(BafGlobals bafGlobals)
    {
        var setting = await dataContext.SiteSettings
            .Where(m => m.Key == DevSettings.Key)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (setting == null)
        {
            bafGlobals.LogStripeActivityStatus = LogStripeActivityStatus.DoLog;
            return;
        }

        bool logActivity = setting.Value.FromJson<DevSettings>().LogAllStripeWebhookActivity;

        if (logActivity) bafGlobals.LogStripeActivityStatus = LogStripeActivityStatus.DoLog;
        else bafGlobals.LogStripeActivityStatus = LogStripeActivityStatus.NoLog;

    }
}
