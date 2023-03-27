namespace AuthTemplateNET7.Server.Services.PaymentServices;

public class StripeAccount
{
    public bool TestModeOn { get; set; } = true;
    public string TestPublishableKey { get; set; }
    public string TestSecretKey { get; set; }
    public string PublishableKey { get; set; }
    public string SecretKey { get; set; }
}
