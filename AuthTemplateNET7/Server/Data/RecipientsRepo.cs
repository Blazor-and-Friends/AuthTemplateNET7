using AuthTemplateNET7.Shared;
using AuthTemplateNET7.Shared.PublicModels;
using Microsoft.EntityFrameworkCore;

namespace AuthTemplateNET7.Server.Data;

public class RecipientsRepo
{
    private readonly DataContext dataContext;

    public RecipientsRepo(DataContext dataContext)
    {
        this.dataContext = dataContext;
    }

    public async Task Unsubscribe(Guid recipientId, string ip)
    {
        var recip = await dataContext.Recipients.FirstOrDefaultAsync(m => m.Id == recipientId);

        if (recip != null)
        {
            recip.Unsubscribed = true;
            _ = dataContext.Update(recip);
        }
        else
        {
            LogItem logItem = new($"An unsubscribe attempt was made with {recipientId} from {ip}", bootstrapColor: BootstrapColor.Warning);
            _ = dataContext.Add(logItem);
        }

        await dataContext.TrySaveAsync($"Could not unsubscribe recipient with Id {recipientId}");
    }
}
