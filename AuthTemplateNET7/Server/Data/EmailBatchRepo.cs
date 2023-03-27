using AuthTemplateNET7.Server.Services.EmailingServices;
using AuthTemplateNET7.Shared;
using AuthTemplateNET7.Shared.PublicModels;
using AuthTemplateNET7.Shared.PublicModels.SiteSettingModels;
using AuthTemplateNET7.Shared.PublicModels.SiteSettingModels.Models.DevSettings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;

namespace AuthTemplateNET7.Server.Data;

public class EmailBatchRepo
{
    private readonly DataContext dataContext;
    private readonly IEmailService emailService;
    EmailSettings emailSettings;
    private readonly LinkHelpers linkHelpers;
    SiteSetting siteSettingEmail;

    /// <summary>
    /// To avoid Admin trying to update a batch currently being processed
    /// </summary>
    public static int CurrentBatchId { get; private set; }

    public EmailBatchRepo(DataContext dataContext, IEmailService emailService, LinkHelpers linkHelpers)
    {
        this.dataContext = dataContext;
        this.emailService = emailService;
        this.linkHelpers = linkHelpers;
    }

    public async Task ProcessBatchesAsync()
    {
        //todo at some point processing batches needs unit testing

        if (await emailingIsOn() == false) return;

        int availableToSendCount = emailSettings.AvailableToSend();

        if (availableToSendCount < 1) return;

        var unfinishedBatches = await dataContext.Batches
            .Where(m => m.BatchStatus == BatchStatus.InProgress)
            .OrderByDescending(m => m.Priority)
            .Include(m => m.Emails.Where(n => n.EmailSendResult == EmailSendResult.Pending)).ToArrayAsync();

        if (unfinishedBatches.Length < 1) return;

        int sentCount = 0;
        foreach (var b in unfinishedBatches)
        {
            EmailBatchRepo.CurrentBatchId = b.Id;

            foreach (var email in b.Emails)
            {
                if (sentCount >= availableToSendCount) break;

                Exception exception = null;
                if (b.AppendUnsubscribeLink && email.RecipientId != null)
                {
                    string unsubscribeLink = linkHelpers.GetUnsubscribeLinkHtml(email.RecipientId.Value);

#pragma warning disable CS0618
                    exception = await emailService.SendAsync(b.Body + unsubscribeLink, b.Subject, email.ToAddress);
#pragma warning restore CS0618
                }
                else
                {
#pragma warning disable CS0618
                    exception = await emailService.SendAsync(b.Body, b.Subject, email.ToAddress);
#pragma warning restore CS0618
                }

                if(exception == null)
                {
                    email.EmailSendResult = EmailSendResult.Success;
                    email.DateSent = DateTime.UtcNow;

                    b.SentEmailsCount++;
                }
                else
                {
                    LogItem logItem = new(exception, $"Could not send email to {email.ToAddress} with subject {b.Subject}. Batch.Id: {b.Id} Email.Id: {email.Id}");
                    _ = dataContext.Add(logItem);

                    email.EmailSendResult = EmailSendResult.Error;

                    b.ErrorsCount++;
                }

                _ = dataContext.Update(email);

                sentCount++;
            }

            DateTime endedSendingTime = DateTime.UtcNow;

            if(b.ErrorsCount + b.SentEmailsCount >= b.TotalEmailsCount)
            {
                b.DateCompleted = endedSendingTime;
                b.BatchStatus = BatchStatus.Complete;
            }

            updateSiteSetting(endedSendingTime, sentCount);

            _ = dataContext.Update(b);

            if (sentCount >= availableToSendCount) break;
        }

        await dataContext.TrySaveAsync($"Could not save a ProcessBatches() run");
    }

    public async Task<EmailSendResult> SendSingleEmailAsync(string body, bool devOnly, string subject, string toAddress, bool appendUnsubscribeLink, Priority priority = Priority.High, string toName = null, Guid? recipientId = null, int deleteAfterDays = 365)
    {
        if (await emailingIsOn() == false) return EmailSendResult.Error;

        if (appendUnsubscribeLink && recipientId == null)
        {
            throw new ArgumentNullException("YOU NEED A recipientId IN ORDER TO APPEND AN UNSUBSCRIBE LINK");
        }

        Batch batch = createBatchWithOneRecipient(appendUnsubscribeLink, body, deleteAfterDays, devOnly, priority, recipientId, subject, toAddress, toName);

        int availableToSendCount = emailSettings.AvailableToSend();

        if(availableToSendCount < 1)
        {
            dataContext.Add(batch);
            _ = await dataContext.TrySaveAsync($"Could not save batch");
            return EmailSendResult.Pending;
        }

#pragma warning disable CS0618
        var exception = await emailService.SendAsync(batch.Body, subject, toAddress);
#pragma warning restore CS0618

        var recip = batch.Emails[0];

        string saveMsg;

        if (exception == null)
        {
            DateTime utcNow = DateTime.UtcNow;

            batch.BatchStatus = BatchStatus.Complete;
            batch.DateCompleted = utcNow;
            batch.SentEmailsCount = 1;

            recip.EmailSendResult = EmailSendResult.Success;
            recip.DateSent = utcNow;

            saveMsg = $"Sent an email to {toAddress}";
        }
        else
        {
            batch.ErrorsCount = 1;
            recip.EmailSendResult = EmailSendResult.Error;
            LogItem logItem = new(exception, $"Could not send email to {toAddress} with body {batch.Body}");
            _ = dataContext.Add(logItem);

            saveMsg = $"Could not send an emal to {toAddress}";
        }

        _ = dataContext.Add(batch);

        updateSiteSetting(DateTime.UtcNow, 1);

        await dataContext.TrySaveAsync(saveMsg + " but did not save the EmailBatch");

        if (exception == null) return EmailSendResult.Success;
        return EmailSendResult.Error;
    }

    Batch createBatchWithOneRecipient(bool appendUnsubscribeLink, string body, int deleteAfterDays, bool devOnly, Priority priority, Guid? recipientId, string subject, string toAddress, string toName)
    {

        Batch result = new() {
            DeleteAfter = DateTime.UtcNow.AddDays(deleteAfterDays),
            DevOnly = devOnly,
            Priority = priority,
            Subject = subject,
            TotalEmailsCount = 1
        };

        if(appendUnsubscribeLink)
        {
            var unsubscribeLink = linkHelpers.GetUnsubscribeLinkHtml(recipientId.Value);

            //wrap in a div to ensure unsubscribe link is on its own line
            result.Body = $"{body}{unsubscribeLink}";
        }
        else
        {
            result.Body = body;
        }

        result.Emails = new()
        {
            new Email
            {
                ToAddress = toAddress,
                ToName = toName,
            }
        };

        return result;
    }

    async Task<bool> emailingIsOn()
    {
        siteSettingEmail = await dataContext.SiteSettings.Where(m => m.Key == EmailSettings.Key).FirstOrDefaultAsync();

        if (siteSettingEmail == null)
        {
            LogItem logItem = new("EmailSettings have not been set up");
            _ = dataContext.Add(logItem);
            await dataContext.TrySaveAsync("Could not add log item to notify EmailSettings not set up");
            return false;
        }

        emailSettings = siteSettingEmail.Value.FromJson<EmailSettings>();
        emailSettings.UpdateTimes();

        return emailSettings.EmailingOn;
    }

    public void updateSiteSetting(DateTime endedSendingTime, int sentCount)
    {
        emailSettings.UpdateAccumulators(endedSendingTime, sentCount);
        siteSettingEmail.Value = emailSettings.ToJson();
        dataContext.Update(siteSettingEmail);
    }
}
