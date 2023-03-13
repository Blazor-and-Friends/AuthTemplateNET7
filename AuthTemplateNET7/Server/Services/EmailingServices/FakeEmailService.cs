using AuthTemplateNET7.Shared;

namespace AuthTemplateNET7.Server.Services.EmailingServices;

//added

public class FakeEmailService : IEmailService
{
#pragma warning disable CS1998
    public async Task<Exception> SendAsync(string body, string subject, string toAddress)
    {
        Random random = new Random();

        var which = random.Next(0, 3);

        if (which == 0)
        {
            Exception e = new Exception("An exception thrown by FakeEmailService");
            return e;
        }
        else
        {
            return null;
        }
    }
#pragma warning restore CS1998

    public void Dispose() { }
}
