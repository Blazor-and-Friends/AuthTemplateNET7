using AuthTemplateNET7.Server.Services.EmailingServices;
using AuthTemplateNET7.Server.Services.PaymentServices;
using AuthTemplateNET7.Shared.Dtos.Payments;
using AuthTemplateNET7.Shared.PublicModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace AuthTemplateNET7.Server.Controllers;

[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    PaymentsRepo paymentsRepo;

    public PaymentsController(DataContext dataContext, BafGlobals bafGlobals)
    {
        paymentsRepo = new PaymentsRepo(dataContext);
    }

    [HttpPost("create-checkout-session")]
    public async Task<IActionResult> CreateCheckoutSession([FromBody]Order model, [FromServices] StripePaymentService stripePaymentService)
    {
        model.OrderStatus = OrderStatus.Checkout;

        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var memberId);

        model.MemberId = memberId;

        await paymentsRepo.VerifyOrderTotalAsync(model);

        bool created = await paymentsRepo.CreateOrUpdateOrderAsync(model);

        if(!created)
        {
            return StatusCode(500, "There was an issue getting you checked out. Try again in a few moments.");
        }

        string memberEmail = User.FindFirstValue(ClaimTypes.Email);

        var session = stripePaymentService.CreateCheckoutSession(model, memberEmail);

        return Ok(new StripeCheckoutSessionWrapper(model, session.Url));
    }

    [AllowAnonymous]
    [HttpPost("stripe-webhook")]
    public async Task<IActionResult> StripeWebhook([FromServices]BafGlobals bafGlobals, [FromServices]EmailBatchRepo emailBatchRepo)
    {
        //install the Stripe CLI: https://stripe.com/docs/stripe-cli#install
        //then open a command prompt in the folder you installed it and run the following to listen for webhooks:
        // stripe listen --forward-to https://localhost:7196/api/payments/stripe-webhook

        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        string stripeWebhooksSecretKey = Environment.GetEnvironmentVariable("STRIPE_WEBHOOKS_SECRET_KEY", EnvironmentVariableTarget.User);

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                stripeWebhooksSecretKey);

            if (stripeEvent.Type == Events.CheckoutSessionCompleted)
            {
                await paymentsRepo.MaybeLogStripeWebhookActivityAsync(bafGlobals, stripeEvent.Type, json, false);

                var session = stripeEvent.Data.Object as Session;

                var guidStr = session.Metadata["orderId"];

                await paymentsRepo.UpdateOrderAsync(guidStr, OrderStatus.Paid);

                decimal amount = session.AmountTotal.HasValue ? session.AmountTotal.Value/100 : 0;

                _ = await emailBatchRepo.SendSingleEmailAsync(
                    body: $"<p>You payed {amount.ToString("C")}.</p>",
                    devOnly: false,
                    subject: "Payment made",
                    toAddress: session.CustomerEmail,
                    appendUnsubscribeLink: false, priority: Shared.Priority.High);
            }
            else
            {
                paymentsRepo.MaybeLogStripeWebhookActivityAsync(bafGlobals, stripeEvent.Type, json, true).Forget();
            }
        }
        catch (Exception e)
        {
            paymentsRepo.CreateLogItemAsync(e, "thrown in PaymentsController.StripeWebhook()").Forget();
        }

        return Ok();
    }
}
