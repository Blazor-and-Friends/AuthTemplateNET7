using AuthTemplateNET7.Shared.Dtos.Dev;

namespace AuthTemplateNET7.Server.Services.EmailingServices.NetMailHelpers;

//added

public class NetMailAccountHelper
{
    string key;

    public NetMailAccount NetMailAccount { get; }

    public NetMailAccountHelper(BafGlobals bafGlobals)
    {
        key = "NET_MAIL_ACCOUNT_" + bafGlobals.AppName;

        NetMailAccount = getStoredAccount();
    }

    public void DeleteAccount()
    {
#if DEBUG
        Environment.SetEnvironmentVariable(key, null, EnvironmentVariableTarget.User);
#else
        Environment.SetEnvironmentVariable(key, null);
#endif
    }

    public NetMailAccountDto GetDto()
    {
        if (NetMailAccount == null) return new();

        return new()
        {
            Address = NetMailAccount.Address,
            Name = NetMailAccount.Name,
            Username = NetMailAccount.Username,
            Password = NetMailAccount.Password,
            EnableSsl = NetMailAccount.EnableSsl,
            Host = NetMailAccount.Host,
            Port = NetMailAccount.Port,
        };
    }

    public NetMailAccountStatusDto GetStatusDto()
    {
        string message = NetMailAccount == null ? "You have not set up a Net.Mail account" : "Your Net.Mail account is set up. Use the form below if you want to test it.";

        return new()
        {
            HaveAccountInfo = NetMailAccount != null,
            StatusMessage = message,
        };
    }

    public void SaveAccount(NetMailAccountDto model)
    {
        NetMailAccount result = new()
        {
            Address = model.Address,
            EnableSsl = model.EnableSsl,
            Host = model.Host,
            Name = model.Name,
            Password = model.Password,
            Port = model.Port,
            Username = model.Username,
        };

#if DEBUG
        Environment.SetEnvironmentVariable(key, result.ToJson(), EnvironmentVariableTarget.User);
#else
        Environment.SetEnvironmentVariable(key, result.ToJson());
#endif
    }

    #region helpers

    NetMailAccount getStoredAccount()
    {
        string value;

#if DEBUG
        value = Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.User); //make it easier to remove old/unused from the Windows interface
#else
        value = Environment.GetEnvironmentVariable(key);
#endif

        if(value == null) return null;

        return value.FromJson<NetMailAccount>();
    }
    #endregion //helpers
}
