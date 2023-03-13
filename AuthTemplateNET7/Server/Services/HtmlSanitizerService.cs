using AuthTemplateNET7.Shared.PublicModels;
using Ganss.Xss;

namespace AuthTemplateNET7.Server.Services;

//added
public class HtmlSanitizerService
{
    HtmlSanitizer htmlSanitizer;

    public string Sanitize(string html)
    {
        if (htmlSanitizer == null) htmlSanitizer = new();

        return htmlSanitizer.Sanitize(html);
    }

    public void SanitizeContactMessage(ContactMessage message)
    {
        message.EmailAddress = Sanitize(message.EmailAddress);
        message.FirstName = Sanitize(message.FirstName);
        message.LastName = Sanitize(message.LastName);
        message.Phone = Sanitize(message.Phone);
        message.Message = Sanitize(message.Message);
        message.Subject = Sanitize(message.Subject);
    }
}
