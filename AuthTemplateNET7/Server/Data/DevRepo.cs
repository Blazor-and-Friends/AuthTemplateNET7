using AuthTemplateNET7.Shared;
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

    //todo DOCS explain my repos and how EF implements the repository pattern and these are really just methods grouped together. I use "repo" as an indicator these touch the database
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

        //todo DOCS make a vid showing how i use trysave/trysaveAsync
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

    #region SiteSettings

    public async Task<EmailSettings> GetEmailSettingsAsync()
    {
        var setting = await dataContext.SiteSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Key == EmailSettings.Key);

        if (setting == null) return new EmailSettings();

        return setting.Value.FromJson<EmailSettings>();
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
            dataContext.Add(setting);
        }
        else
        {
            setting.Value = model.ToJson();
            dataContext.Update(setting);
        }

        return await dataContext.TrySaveAsync($"Could not save email settings:\r\n{model.ToJson(true)}") > 0;
    }

    public async Task<bool> UpdateSettingsAsync(DevSettings model)
    {
        var setting = await dataContext.SiteSettings.FirstOrDefaultAsync(m => m.Key == DevSettings.Key);

        if(setting == null)
        {
            setting = new(DevSettings.Key, RoleLevel.Dev, model);
            dataContext.Add(setting);
        }
        else
        {
            setting.Value = model.ToJson();
            dataContext.Update(setting);
        }

        return await dataContext.TrySaveAsync($"Could not save dev settings: \r\n: {model.ToJson(true)}") > 0;
    }

    #endregion //SiteSettings
}
