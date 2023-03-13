using Microsoft.AspNetCore.Components;

namespace AuthTemplateNET7.Client.Shared;

//added
public static class MarkupHelpers
{
    /// <summary>
    /// Renders raw HTML into a view/page/component
    /// </summary>
    /// <param name="htmlString">The HTML string you want to render unescaped</param>
    /// <returns>Raw HTML</returns>
    public static MarkupString ToMarkupString(this string htmlString)
    {
        return new MarkupString(htmlString);
    }

    public static MarkupString ToEmailLink(this string emailAddress)
    {
        if (string.IsNullOrWhiteSpace(emailAddress)) return (MarkupString)null;

        return $"<a href='mailto:{emailAddress}'>{emailAddress}</a>".ToMarkupString();
    }

    public static MarkupString ToIpLookupLink(this string ip, bool openInNewTab = true)
    {
        if (string.IsNullOrWhiteSpace(ip)) return (MarkupString)null;

        string targetBlank = openInNewTab ? " target='_blank'" : "";
        //give Bing a little help
        return $"<a href='https://www.bing.com/search?q={ip}'{targetBlank}>{ip}</a>".ToMarkupString();
    }
}
