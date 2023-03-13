using AuthTemplateNET7.Shared;
using AuthTemplateNET7.Shared.Dtos.Admin;
using AuthTemplateNET7.Shared.PublicModels;
using AuthTemplateNET7.Shared.PublicModels.SiteSettingModels;
using AuthTemplateNET7.Shared.PublicModels.SiteSettingModels.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthTemplateNET7.Server.Data;

//added
public class AdminRepo
{
    private readonly DataContext dataContext;

    public AdminRepo(DataContext dataContext)
    {
        this.dataContext = dataContext;
    }

    public async Task<DashboardPageModel> GetDashboardPageModelAsync(string appKey)
    {
        //todo a view in sql server

        var contactMessagesCount = await dataContext.ContactMessages.CountAsync();

        var failedLoginsCount = await dataContext.Logins.Where(m => !m.Success).CountAsync();

        return new DashboardPageModel(contactMessagesCount, failedLoginsCount);
    }

    #region contact messages

    public async Task<ContactMessage[]> GetContactMessagesAsync()
    {
        var result = await dataContext.ContactMessages
            .OrderByDescending(m => m.DateTime)
            .ToArrayAsync();

        foreach (var item in result)
        {
            if (item.SaveMessage)
            {
                item.Disposition = Disposition.Save;
                item.DispositionLabel = ContactMessage.Labels[2];
            }
            else item.DispositionLabel = ContactMessage.Labels[0];
        }

        return result;
    }

    public async Task<(int savedCount, int deletedCount, int rows)> UpdateContactMessages(ContactMessage[] arr)
    {
        int savedResult = 0;
        int deletedResult = 0;
        foreach (var item in arr)
        {
            if(item.Disposition == Disposition.Save)
            {
                item.SaveMessage = true;
                dataContext.Update(item);
                savedResult++;
            }
            else if(item.Disposition == Disposition.DeleteNow)
            {
                dataContext.Remove(item);
                deletedResult++;
            }
            else if(item.Disposition == Disposition.AutoDelete && item.SaveMessage)
            {
                item.SaveMessage = false;
                dataContext.Update(item);
            }
        }

        var rows = await dataContext.TrySaveAsync($"Could not delete {deletedResult}  nor mark as saved {savedResult} ContactMessages");

        return (savedResult, deletedResult, rows);
    }

    #endregion //contact messages

    #region email batches

    public Task<int> DeleteEmailBatchesByIds(int[] ids)
    {
#if DEBUG
        var all = dataContext.EmailBatches.ToArray();

        var idsDict = ids.ToDictionary(k => k, v => false);

        foreach (var item in all)
        {
            if (idsDict.ContainsKey(item.Id)) dataContext.Remove(item);
        }

        var rows = dataContext.TrySave($"Could not delete {ids.Length} EmailBatches");

        return Task.FromResult(rows);
#else
        return dataContext.EmailBatches
            .Where(m => ids.Any(id => m.Id == id))
            .ExecuteDeleteAsync();
#endif
    }

    public Task<EmailBatch[]> GetEmailBatchesAsync()
    {
        return dataContext.EmailBatches
            .Where(m => !m.DevOnly)
            .OrderByDescending(m => m.DateCreated)
            .AsNoTracking()
            .ToArrayAsync();
    }

    public Task<Email[]> GetEmailsForBatch(int batchId)
    {
        return dataContext.Emails
            .Where(m => m.EmailBatchId == batchId)
            .OrderBy(m => m.ToAddress)
            .AsNoTracking()
            .ToArrayAsync();
    }

    public async Task<bool> PauseOrResumeBatchAsync(EmailBatch batch)
    {
        //todo at some point we're gonna need to manage handling a batch that is currently having emails sent out since when a portion of emails are sent and the batch is resaved to the db, it will overwrite the "paused" status. A quick and dirty way to do this would be to put a static Property on EmailBatchRepo for the batch currently in progress and check that the two aren't the same

        bool needsUpdating = false;
        if (batch.BatchStatus == BatchStatus.InProgress)
        {
            batch.BatchStatus = BatchStatus.Paused;
            needsUpdating = true;
        }
        else if (batch.BatchStatus == BatchStatus.Paused)
        {
            batch.BatchStatus = BatchStatus.InProgress;
            needsUpdating = true;
        }

        if (needsUpdating)
        {
            dataContext.Update(batch);
            var rows = await dataContext.TrySaveAsync($"Could not change BatchStatus on {batch.Subject} with Id {batch.Id}");
            if (rows > 0) return true;
        }

        return false;
    }

    #endregion //email batches

    #region recipients

    public async Task<(CreateRecipientResult result, Recipient recipient)> CreateRecipient(Recipient model)
    {
        if(await dataContext.Recipients.AnyAsync(m => m.Address == model.Address))
        {
            var existing = await dataContext.Recipients.FirstOrDefaultAsync(m => m.Address == model.Address);

            return (CreateRecipientResult.RecipientExists, existing);
        }

        dataContext.Add(model);

        var rows = await dataContext.TrySaveAsync($"Could not create recipient {model.ToJson(true)}");
        if (rows > 0) return (CreateRecipientResult.Success, model);

        return (CreateRecipientResult.ServerError, null);
    }

    public Task<Recipient[]> GetRecipients()
    {
        return dataContext.Recipients.OrderBy(m => m.Address).ToArrayAsync();
    }

    public async Task<bool> UpdateRecipient(Recipient model)
    {
        dataContext.Update(model);

        return await dataContext.TrySaveAsync($"Could not update recipient: {model.ToJson(true)}") != -1;
    }

    #endregion //recipients

    #region settings

    public async Task<AdminSettings> GetSettings()
    {
        var setting = await dataContext.SiteSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Key == AdminSettings.Key);

        if (setting == null) return new AdminSettings();

        return setting.Value.FromJson<AdminSettings>();
    }

    public async Task<bool> SaveSettingsAsync(AdminSettings model)
    {
        var setting = await dataContext.SiteSettings
            .FirstOrDefaultAsync(m => m.Key == AdminSettings.Key);

        if (setting == null)
        {
            setting = new(AdminSettings.Key, RoleLevel.Admin, model);
            dataContext.Add(setting);
        }
        else
        {
            setting.Value = model.ToJson();
            dataContext.Update(setting);
        }

        return await dataContext.TrySaveAsync($"Could not save admin settings: \r\n{model.ToJson(true)}") > 0;
    }

    #endregion //settings
}

public enum CreateRecipientResult
{
    Success,
    RecipientExists,
    ServerError
}
