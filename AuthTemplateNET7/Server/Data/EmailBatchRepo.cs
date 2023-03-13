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
    private readonly LinkHelpers linkHelpers;

    public EmailBatchRepo(DataContext dataContext, IEmailService emailService, LinkHelpers linkHelpers)
    {
        this.dataContext = dataContext;
        this.emailService = emailService;
        this.linkHelpers = linkHelpers;
    }

    public async Task ProcessBatches()
    {
        var setting = await dataContext.SiteSettings.Where(m => m.Key == EmailSettings.Key).FirstOrDefaultAsync();

        if (setting == null)
        {
            LogItem logItem = new("EmailSettings have not been set up");
            dataContext.Add(logItem);
            await dataContext.TrySaveAsync("Could not add log item to notify EmailSettings not set up");
            return;
        }

        EmailSettings emailSettings = setting.Value.FromJson<EmailSettings>();

        if (!emailSettings.EmailingOn) return;

        int availableToSendCount = emailSettings.AvailableToSend();

        if (availableToSendCount < 1) return;

        var unfinishedBatches = await dataContext.EmailBatches
            .Where(m => m.BatchStatus == BatchStatus.InProgress)
            .OrderByDescending(m => m.Priority)
            .Include(m => m.Emails.Where(n => n.EmailSendResult == EmailSendResult.Pending)).ToArrayAsync();

        if (unfinishedBatches.Length < 1) return;

        int sentCount = 0;
        foreach (var b in unfinishedBatches)
        {
            sentCount++;
            if(sentCount >= availableToSendCount)
            {



                dataContext.Update(b);
                break;
            }

            var emails = b.Emails;
        }

        //todo send out emails
    }

    public async Task<bool> SendSingleEmailAsync(string body, string subject, string toAddress, bool appendUnsubscribeLink, Priority priority = Priority.High, string toName = null, Guid? recipientId = null, int deleteAfterDays = 365)
    {
        //todo at some point If your emailing services rate limits you, you'll need to set up checks. Especially if you're using System.Net.Mail.SmtpClient as there will be no indication the send attempt was rejected.

        if(appendUnsubscribeLink && recipientId == null)
        {
            throw new ArgumentNullException("YOU NEED A recipientId IN ORDER TO APPEND AN UNSUBSCRIBE LINK");
        }

        EmailBatch batch = createBatchWithOneRecipient(appendUnsubscribeLink, body, deleteAfterDays, priority, recipientId, subject, toAddress, toName);

#pragma warning disable CS0618
        var exception = await emailService.SendAsync(batch.Body, subject, toAddress);
#pragma warning restore CS0618

        var recip = batch.Emails[0];

        string saveMsg;

        if (exception != null)
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
            dataContext.Add(logItem);

            saveMsg = $"Could not send an emal to {toAddress}";
        }

        dataContext.Add(batch);

        await dataContext.TrySaveAsync(saveMsg + " but did not save the EmailBatch");

        return exception == null;
    }

    EmailBatch createBatchWithOneRecipient(bool appendUnsubscribeLink, string body, int deleteAfterDays, Priority priority, Guid? recipientId, string subject, string toAddress, string toName)
    {

        EmailBatch result = new() {
            DeleteAfter = DateTime.UtcNow.AddDays(deleteAfterDays),
            Priority = priority,
            Subject = subject,
            TotalEmailsCount = 1
        };

        if(appendUnsubscribeLink)
        {
            var unsubscribeLink = linkHelpers.GetUnsubscribeLinkHtml(recipientId.Value);

            //wrap in a div to ensure unsubscribe link is on its own line
            result.Body = $"{body}{linkHelpers}";
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
}
