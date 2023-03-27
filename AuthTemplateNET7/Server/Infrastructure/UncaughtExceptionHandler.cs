using AuthTemplateNET7.Shared.PublicModels;
using AuthTemplateNET7.Shared.PublicModels.SiteSettingModels.Models.DevSettings;
using Microsoft.EntityFrameworkCore;

namespace AuthTemplateNET7.Server.Infrastructure;

public class UncaughtExceptionHandler
{

    public async Task LogUnhandledException(Exception exception, DataContext ctx, BafGlobals bafGlobals, LinkHelpers linkHelpers)
    {
        LogItem logItem = new(exception, "Caught in Server.Infrastructure.UncaughtExceptionHandler");

        ctx.Add(logItem);

        var setting = await ctx.SiteSettings
            .AsNoTracking()
            .Where(m => m.Key == DevSettings.Key)
            .FirstOrDefaultAsync();

        if (setting != null)
        {
            DevSettings devSettings = setting.Value.FromJson<DevSettings>();

            if(devSettings.SendNotificationsTo != null
                && devSettings.NotifyWhenUncaughtExceptionThrown)
            {
                Email sentEmail = new(devSettings.SendNotificationsTo);

                string body = $"<p><b>Exception message:</b>&nbsp;{logItem.ErrorMessage}. <b>Stacktrace:</b></p><pre>{logItem.StackTraceOrJson}</pre><p>Check your {linkHelpers.GetLogItemsPageLink()}.</p>"; //log item has innerexception message

                Batch emailBatch = new(
                    body,
                    deleteAfterDays: 7,
                    devOnly: true,
                    sentEmail,
                    subject: $"Uncaught exception thrown from {bafGlobals.AppName}");

                ctx.Add(emailBatch);
            }
        }

        await ctx.TrySaveAsync("Could not log unhandled exception");
    }
}
