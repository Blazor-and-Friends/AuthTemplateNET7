using AuthTemplateNET7.Shared.Dtos.Dev;

namespace AuthTemplateNET7.Server.Services.PaymentServices;

public class StripeAccountHelper
{
    string key;

    public StripeAccount StripeAccount { get; set; }

    public StripeAccountHelper(BafGlobals bafGlobals)
    {
        key = "STRIPE_KEYS_" + bafGlobals.AppName;
        StripeAccount = getStoredAccount();
    }

    public void DeleteAccount()
    {
#if DEBUG
        Environment.SetEnvironmentVariable(key, null, EnvironmentVariableTarget.User);
#else
        Environment.SetEnvironmentVariable(key, null);
#endif
    }

    public StripeAccountDto GetDto()
    {
        if (StripeAccount == null) return new();

        return new()
        {
            TestMode = StripeAccount.TestModeOn,
            TestPublishableKey = StripeAccount.TestPublishableKey,
            TestSecretKey = StripeAccount.TestSecretKey,
            PublishableKey = StripeAccount.PublishableKey,
            SecretKey = StripeAccount.SecretKey
        };
    }

    public void SaveAccount(StripeAccountDto model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(StripeAccountDto) + " CAN NOT BE NULL");
        }

        StripeAccount result = new()
        {
            TestModeOn = model.TestMode,
            TestPublishableKey = model.TestPublishableKey,
            TestSecretKey = model.TestSecretKey,
            PublishableKey = model.PublishableKey,
            SecretKey = model.SecretKey,
        };

#if DEBUG
        Environment.SetEnvironmentVariable(key, result.ToJson(), EnvironmentVariableTarget.User); //make it easier to remove old/unused from the Windows interface
#else
        Environment.SetEnvironmentVariable(key, result.ToJson());
#endif
    }

    #region helpers

    StripeAccount getStoredAccount()
    {
        string value;

#if DEBUG
        value = Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.User); //make it easier to remove old/unused from the Windows interface
#else
        value = Environment.GetEnvironmentVariable(key);
#endif

        if (value == null) return null;

        return value.FromJson<StripeAccount>();
    }

    #endregion //helpers
}
