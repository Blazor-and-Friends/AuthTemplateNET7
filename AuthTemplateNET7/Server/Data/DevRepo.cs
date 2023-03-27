using AuthTemplateNET7.Server.Services.EmailingServices.NetMailHelpers;
using AuthTemplateNET7.Server.Services.PaymentServices;
using AuthTemplateNET7.Shared;
using AuthTemplateNET7.Shared.Dtos.Dev;
using AuthTemplateNET7.Shared.PublicModels;
using AuthTemplateNET7.Shared.PublicModels.SiteSettingModels;
using AuthTemplateNET7.Shared.PublicModels.SiteSettingModels.Models.DevSettings;
using AuthTemplateNET7.Shared.SharedServices;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text;

namespace AuthTemplateNET7.Server.Data;

//added
public class DevRepo
{
    DataContext dataContext;

    public DevRepo(DataContext dataContext)
    {
        this.dataContext = dataContext;
    }

    public Task<Login[]> GetFailedLogins()
    {
        return dataContext.Logins
            .Where(m => m.Success == false)
            .OrderByDescending(m => m.DateTime)
            .AsNoTracking()
            .ToArrayAsync();
    }

    #region LogItems

    /// <summary>
    /// Deletes all LogItems
    /// </summary>
    /// <returns>Number of LogItems deleted</returns>
    public Task<int> DeleteAllLogItems()
    {

#if DEBUG
        var logItems = dataContext.LogItems.ToArray();
        dataContext.RemoveRange(logItems);
        var rows = dataContext.TrySave("Could not delete all LogItems");
        return Task.FromResult(rows);
#else
        //https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-7.0/whatsnew#executeupdate-and-executedelete-bulk-updates
        return dataContext.Logins.ExecuteDeleteAsync();
#endif
    }

    public Task<int> DeleteLogItemsById(int[] ids)
    {
        //todo SQL make sure AuthTemplateNET7.Server.Data.DevRepo.DeleteLogItemsById(int[] ids) works against sql server
#if DEBUG
        var allLogItems = dataContext.LogItems.ToArray();

        Dictionary<int, bool> idsDict = ids.ToDictionary(k => k, v => false);

        foreach (var item in allLogItems)
        {
            if (idsDict.ContainsKey(item.Id)) dataContext.Remove(item);
        }

        var rows = dataContext.TrySave($"Could not delete {ids.Length} log items");

        return Task.FromResult(rows);
#else
        return dataContext.LogItems
            .Where(m => ids.Any(id => id == m.Id))
            .ExecuteDeleteAsync();
#endif
    }

    public Task<LogItem[]> GetLogItems()
    {
        return dataContext.LogItems
            .OrderByDescending(m => m.DateTime)
            .AsNoTracking()
            .ToArrayAsync();
    }

    #endregion //LogItems

    #region Net.Mail account

    public void DeleteNetMailAccountInfo(BafGlobals bafGlobals)
    {
        new NetMailAccountHelper(bafGlobals).DeleteAccount();
    }

    public NetMailAccountStatusDto GetNetMailAccountStatusDto(BafGlobals bafGlobals)
    {
        return new NetMailAccountHelper(bafGlobals).GetStatusDto();
    }

    public async Task<(NetMailAccountDto dto, string errorMessage)> GetNetMailAccountAsync(BafGlobals bafGlobals, Guid memberId)
    {
        var member = await dataContext.Members.FindAsync(memberId);

        if (member.PasswordVerifitedExpiration == null)
        {
            return (null, "Hmm...");
        }

        if (DateTime.UtcNow > member.PasswordVerifitedExpiration.Value)
        {
            return (null, "Password verification token expired.");
        }

        member.PasswordVerifitedExpiration = null;
        dataContext.Update(member);
        _ = await dataContext.TrySaveAsync();

        var dto = new NetMailAccountHelper(bafGlobals).GetDto();

        return (dto, null);
    }


    #endregion //Net.Mail account

    #region SiteSettings

    public async Task<EmailSettings> GetEmailSettingsAsync()
    {
        var setting = await dataContext.SiteSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Key == EmailSettings.Key);

        if (setting == null) return new EmailSettings();

        var result = setting.Value.FromJson<EmailSettings>();
        result.UpdateTimes();
        return result;
    }

    public async Task<DevSettings> GetSettingsAsync()
    {
        var setting = await dataContext.SiteSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Key == DevSettings.Key);

        if(setting == null) return new DevSettings();

        return setting.Value.FromJson<DevSettings>();
    }

    public async Task<bool> UpdateEmailSettingsAsync(EmailSettings model)
    {
        var setting = await dataContext.SiteSettings.FirstOrDefaultAsync(m => m.Key == EmailSettings.Key);

        if (setting == null)
        {
            setting = new(EmailSettings.Key, RoleLevel.Dev, model);
            _ = dataContext.Add(setting);
        }
        else
        {
            setting.Value = model.ToJson();
            _ = dataContext.Update(setting);
        }

        return await dataContext.TrySaveAsync($"Could not save email settings:\r\n{model.ToJson(true)}") > 0;
    }

    public async Task<bool> UpdateSettingsAsync(DevSettings model)
    {
        var setting = await dataContext.SiteSettings.FirstOrDefaultAsync(m => m.Key == DevSettings.Key);

        if(setting == null)
        {
            setting = new(DevSettings.Key, RoleLevel.Dev, model);
            _ = dataContext.Add(setting);
        }
        else
        {
            setting.Value = model.ToJson();
            _ = dataContext.Update(setting);
        }

        return await dataContext.TrySaveAsync($"Could not save dev settings: \r\n: {model.ToJson(true)}") > 0;
    }

    #endregion //SiteSettings

    #region Stripe Account

    public StripeAccountStatusDto CheckStripeAccountStatus(BafGlobals bafGlobals)
    {
        var account = new StripeAccountHelper(bafGlobals).StripeAccount;

        string liveKeysStatus;
        if (!string.IsNullOrWhiteSpace(account.PublishableKey)
            && !string.IsNullOrWhiteSpace(account.SecretKey))
        {
            liveKeysStatus = "Your live keys are stored";
        }
        else
        {
            liveKeysStatus = "You have not set your live keys";
        }

        string testKeysStatus;
        if (!string.IsNullOrWhiteSpace(account.TestPublishableKey)
            && !string.IsNullOrWhiteSpace(account.TestSecretKey))
        {
            testKeysStatus = "Your test keys are stored";
        }
        else
        {
            testKeysStatus = "You have not set your test keys";
        }

        return new()
        {
            LiveKeysStatus = liveKeysStatus,
            TestKeysStatus = testKeysStatus,
            TestModeStatus = account.TestModeOn ? "You are in TEST mode" : "You are LIVE",
        };
    }

    public async Task<(StripeAccountDto dto, string errorMessage)> GetStripeAccountAsync(BafGlobals bafGlobals, Guid memberId)
    {
        var member = await dataContext.Members.FindAsync(memberId);

        if (member.PasswordVerifitedExpiration == null)
        {
            return (null, "Hmm...");
        }

        if (DateTime.UtcNow > member.PasswordVerifitedExpiration.Value)
        {
            return (null, "Password verification token expired.");
        }

        member.PasswordVerifitedExpiration = null;
        dataContext.Update(member);
        _ = await dataContext.TrySaveAsync();

        var dto = new StripeAccountHelper(bafGlobals).GetDto();

        return (dto, null);
    }

    public void SetStripeAccount(BafGlobals bafGlobals, StripeAccountDto model)
    {
        if (model.DeleteAccount)
        {
            new StripeAccountHelper(bafGlobals).DeleteAccount();
            return;
        }

        new StripeAccountHelper(bafGlobals).SaveAccount(model);
    }

    #endregion //Stripe Account
}
