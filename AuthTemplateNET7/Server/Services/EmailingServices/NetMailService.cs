using AuthTemplateNET7.Server.Services.EmailingServices.NetMailHelpers;
using System.Net;
using System.Net.Mail;

namespace AuthTemplateNET7.Server.Services.EmailingServices;

//added

/// <summary>
/// Uses System.Net.Mail to send emails
/// </summary>
public class NetMailService : IEmailService
{
    MailAddress fromAddress_;
    SmtpClient smtpClient_;

    public NetMailService(BafGlobals bafGlobals)
    {
        NetMailAccount netMailAccount = new NetMailAccountHelper(bafGlobals).NetMailAccount;

        if(netMailAccount == null)
        {
            throw new ArgumentNullException("NetMailAccount DOES NOT EXIST IN ENVIRONMENT VARIABLES. YOU CAN SET THIS UP IN Dev > Dashboard > .Net.Mail account");
        }

        smtpClient_ = new(netMailAccount.Host, netMailAccount.Port);
        smtpClient_.EnableSsl = netMailAccount.EnableSsl;
        NetworkCredential networkCredential = new(netMailAccount.Username, netMailAccount.Password);
        smtpClient_.Credentials = networkCredential;

        fromAddress_ = new(address: netMailAccount.Address, displayName: netMailAccount.Name);
    }

    public async Task<Exception> SendAsync(string body, string subject, string toAddress)
    {

        //todo set up requiring email address verification when someone registers for an account. Make sure you have a captcha alternative on the register form: https://www.experienceux.co.uk/ux-blog/5-alternatives-to-captcha-that-wont-baffle-or-frustrate-users/#:~:text=5%20alternatives%20to%20CAPTCHA%20that%20won%E2%80%99t%20baffle%20or,4.%20Simple%20questions%20...%205%205.%20Gamification%20

        MailAddress toMailAddress;
        try
        {
            toMailAddress = new(toAddress);
        }
        catch (Exception e)
        {
            return e;
        }

        MailMessage message = new(fromAddress_, toMailAddress);

        message.Body = body;
        message.Subject = subject;
        message.IsBodyHtml = true;

        try
        {
            await smtpClient_.SendMailAsync(message);
            message.Dispose();
            return null;
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public void Dispose()
    {
        if(smtpClient_ != null) smtpClient_.Dispose();
    }
}
