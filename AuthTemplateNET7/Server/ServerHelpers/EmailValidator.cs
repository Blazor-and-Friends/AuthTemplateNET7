namespace AuthTemplateNET7.Server.ServerHelpers;

public static class EmailValidator
{
    public static bool IsValidEmailAddress(this string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return false;

        try
        {
            System.Net.Mail.MailAddress m = new System.Net.Mail.MailAddress(s);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
