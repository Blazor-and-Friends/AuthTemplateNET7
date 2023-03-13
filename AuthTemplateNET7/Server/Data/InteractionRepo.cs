using AuthTemplateNET7.Server.Services.EmailingServices;
using AuthTemplateNET7.Shared.Dtos.Public;
using AuthTemplateNET7.Shared.PublicModels;
using AuthTemplateNET7.Shared.PublicModels.SiteSettingModels.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthTemplateNET7.Server.Data;

public class InteractionRepo
{
    private readonly DataContext dataContext;

    public InteractionRepo(DataContext dataContext)
    {

        this.dataContext = dataContext;
    }

    #region contact message

    public async Task<bool> CreateContactMessageAsync(ContactMessage model, LinkHelpers linkHelpers)
    {
        new HtmlSanitizerService().SanitizeContactMessage(model);

        createRecipientIfJoiningEmailList(model);

        dataContext.Add(model);

        await notifyAdminIfSettingOnAsync(linkHelpers, model);

        var rows = await dataContext.TrySaveAsync($"Could not save contact message: {model.ToJson(true)}");

        if (rows > 0) return true;
        return false;
    }

    void createRecipientIfJoiningEmailList(ContactMessage contactMessage)
    {
        if (!contactMessage.AddToEmailList
            || !contactMessage.EmailAddress.IsValidEmailAddress())
        {
            return;
        }

        Recipient recipient = new()
        {
            Address = contactMessage.EmailAddress,
            FirstName = contactMessage.FirstName,
            LastName = contactMessage.LastName,
            Source = "Contact Page"
        };

        dataContext.Add(recipient);
    }

    async Task notifyAdminIfSettingOnAsync(LinkHelpers linkHelpers, ContactMessage contactMessage)
    {
        var setting = await dataContext.SiteSettings.Where(m => m.Key == AdminSettings.Key).AsNoTracking().FirstOrDefaultAsync();

        if (setting != null)
        {
            AdminSettings adminSettings = setting.Value.FromJson<AdminSettings>();

            if (adminSettings.NotifyMeWhenThereIsANewContactMessage && adminSettings.SendNotificationsTo != null)
            {
                Email sentEmail = new(adminSettings.SendNotificationsTo);

                string body = $"<p><b>{contactMessage.Subject}</b></p><p>{contactMessage.Message}</p><p>View all {linkHelpers.GetContactMessagesPageLink()}.</p>";
                EmailBatch emailBatch = new(
                    body,
                    deleteAfterDays: 7,
                    devOnly: false,
                    sentEmail,
                    subject: "New message");

                dataContext.Add(emailBatch);
            }
        }
    }

    #endregion //contact message

    public async Task<bool> MaybeAddToEmailList(JoinEmailListDto model, string ip)
    {
        if(!string.IsNullOrEmpty(model.FirstName)
            || !string.IsNullOrEmpty(model.LastName)
            || model.SecondsToSubmit < 4)
        {
            //likely a bot
            LogItem logItem = new($"Looks like a bot tried to join the email list from <a href='https://www.bing.com/search?q={ip}' target='_blank'>{ip}</a>:<br /> {model.ToJson(true)}");

            dataContext.Add(logItem);
            await dataContext.TrySaveAsync();

            return true;
        }

        string emailAddress = new HtmlSanitizerService().Sanitize(model.EmailAddress);

        Recipient recipient = new()
        {
            Address = emailAddress,
            Source = model.Source.Shorten(32)
        };

        dataContext.Add(recipient);

        return (await dataContext.TrySaveAsync($"Could not add email list recipient with address {emailAddress}")) > 0;
    }
}
