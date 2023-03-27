using AuthTemplateNET7.Shared.PublicModels;
using Stripe;
using Stripe.Checkout;

namespace AuthTemplateNET7.Server.Services.PaymentServices;

//added

public class StripePaymentService
{
    private readonly LinkHelpers linkHelpers;
    public StripePaymentService(BafGlobals bafGlobals, LinkHelpers linkHelpers)
    {
        var account = new StripeAccountHelper(bafGlobals).StripeAccount;

        if (account.TestModeOn)
        {
            StripeConfiguration.ApiKey = account.TestSecretKey;
        }
        else
        {
            StripeConfiguration.ApiKey = account.SecretKey;
        }

        this.linkHelpers = linkHelpers;
    }

    public Session CreateCheckoutSession(Order model, string customerEmail)
    {

        var orderItems = model.OrderItems;

        Dictionary<string, string> orderIdDict = new Dictionary<string, string>
        {
            { "orderId", model.Id.ToString() }
        };

        var lineItems = new List<SessionLineItemOptions>(orderItems.Count);

        foreach(var item in orderItems)
        {
            SessionLineItemOptions sessionLineItemOptions = new() { Quantity = item.Quantity };

            SessionLineItemPriceDataOptions priceData = new()
            {
                UnitAmountDecimal = item.LineTotal * 100,
                Currency = "usd"
            };

            SessionLineItemPriceDataProductDataOptions productData = new()
            {
                Name = item.Description
            };


            if (item.ImgUrl != null)
            {
                productData.Images = new List<string> { item.ImgUrl };
            }

            priceData.ProductData = productData;

            sessionLineItemOptions.PriceData = priceData;

            lineItems.Add(sessionLineItemOptions);
        }

        var options = new SessionCreateOptions
        {
            CustomerEmail = customerEmail,

            //PaymentMethodTypes = new List<string>
            //{
            //    "card"
            //},

            LineItems = lineItems,

            Metadata = orderIdDict,

            Mode = "payment", //this can be recurring, see stripe docs

            SuccessUrl = linkHelpers.CreateLocalUrl($"thank-you/{model.Id}"),
            CancelUrl = linkHelpers.CreateLocalUrl("shopping-cart")

        };

        var service = new SessionService();
        Session session = service.Create(options);
        return session;
    }
}
