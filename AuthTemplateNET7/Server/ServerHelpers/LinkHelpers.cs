using AuthTemplateNET7.Shared.PublicModels;

namespace AuthTemplateNET7.Server.ServerHelpers;

//added

public class LinkHelpers
{
    string protocolAndHost;

    public LinkHelpers(IHttpContextAccessor httpContextAccessor)
    {
        var protocol = httpContextAccessor.HttpContext.Request.Scheme;
        var host = httpContextAccessor.HttpContext.Request.Host.Value;
        protocolAndHost = $"{protocol}://{host}";
    }

    /// <summary>
    /// Creates an HTML anchor tag
    /// </summary>
    /// <param name="relativeUrl">Do not include a leading slash</param>
    /// <param name="linkText">The inner HTML of the anchor tag</param>
    /// <returns>HTML anchor tag</returns>
    public string CreateLink(string relativeUrl, string linkText)
    {
        return $"<a href='{protocolAndHost}/{relativeUrl}'>{linkText}</a>";
    }

    /// <summary>
    /// Creates a local URL
    /// </summary>
    /// <param name="relativeUrl">Do not include a leading slash</param>
    /// <returns>Local URL</returns>
    public string CreateLocalUrl(string relativeUrl)
    {
        return $"{protocolAndHost}/{relativeUrl}";
    }

    public string GetContactMessagesPageLink()
    {
        return CreateLink("admin/contact-messages", "contact messages");
    }

    public string GetLogItemsPageLink()
    {
        return CreateLink("dev/log-items", "log items");
    }

    public string GetUnsubscribeLinkHtml(Guid recipientId)
    {
        var url = $"{protocolAndHost}/unsubscribe/{recipientId}";

        string anchorTag = CreateLink($"unsubscribe/{recipientId}", "Unsubscribe");
        //wrap in a div to ensure unsubscribe link is on its own line
        return $"<div>{anchorTag}</div>";
    }
}
