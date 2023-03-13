using AuthTemplateNET7.Shared.PublicModels;

namespace AuthTemplateNET7.Server.Services.EmailingServices;

public class LinkHelpers
{
    string protocolAndHost;

    public LinkHelpers(IHttpContextAccessor httpContextAccessor)
    {
        var protocol = httpContextAccessor.HttpContext.Request.Scheme;
        var host = httpContextAccessor.HttpContext.Request.Host.Value;
        protocolAndHost = $"{protocol}://{host}";
    }

    public string GetContactMessagesPageLink()
    {
        return createLink("admin/contact-messages", "contact messages");
    }

    public string GetLogItemsPageLink()
    {
        return createLink("dev/log-items", "log items");
    }

    public string GetUnsubscribeLinkHtml(Guid recipientId)
    {
        var url = $"{protocolAndHost}/unsubscribe/{recipientId}";

        string anchorTag = createLink($"unsubscribe/{recipientId}", "Unsubscrbe");
        //wrap in a div to ensure unsubscribe link is on its own line
        return $"<div>{anchorTag}</div>";
    }

    string createLink(string relativeUrl, string linkText)
    {
        return $"<a href='{protocolAndHost}/{relativeUrl}'>{linkText}</a>";
    }
}
