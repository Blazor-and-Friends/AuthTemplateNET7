using AuthTemplateNET7.Shared.Dtos.Dev;

namespace AuthTemplateNET7.Server.Services.EmailingServices.NetMailHelpers;

//added

public class NetMailAccountHelper
{
    string key;

    public bool AccountIsStored { get; set; }

    public NetMailAccount NetMailAccount { get; }

    public NetMailAccountHelper(BafGlobals bafGlobals)
    {
        key = "NET_MAIL_ACCOUNT_" + bafGlobals.AppName;

        NetMailAccount = getStoredAccount();
    }

    public void DeleteAccount()
    {
#if DEBUG
        Environment.SetEnvironmentVariable(key, null, EnvironmentVariableTarget.User); //make it easier to remove old/unused from the Windows interface
#else
        Environment.SetEnvironmentVariable(key, null);
#endif
    }

    public NetMailAccountDto GetDto()
    {
        if (NetMailAccount == null) return new();

        return new()
        {
            AccountIsStored = true //no need to expose the account
        };
    }

    public string SaveAccount(NetMailAccountDto model)
    {
        //short-circuit saving if developer is on the page only to send a sample email
        if (
            string.IsNullOrWhiteSpace(model.Address)
            || string.IsNullOrWhiteSpace(model.Host)
            || string.IsNullOrWhiteSpace(model.Name)
            || string.IsNullOrWhiteSpace(model.Password)
            || model.Port < 1
            || string.IsNullOrWhiteSpace(model.Username)
            )
        {
            return null;
        }

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
        Environment.SetEnvironmentVariable(key, result.ToJson(), EnvironmentVariableTarget.User); //make it easier to remove old/unused from the Windows interface
#else
        Environment.SetEnvironmentVariable(key, result.ToJson());
#endif

        return "Account created/updated";
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

        AccountIsStored = true;

        return value.FromJson<NetMailAccount>();
    }
    #endregion //helpers
}
