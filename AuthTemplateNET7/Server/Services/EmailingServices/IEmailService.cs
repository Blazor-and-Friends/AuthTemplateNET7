using AuthTemplateNET7.Shared;

namespace AuthTemplateNET7.Server.Services.EmailingServices;

public interface IEmailService : IDisposable
{
    [Obsolete("Use Server.Data.EmailBatchRepo.SendSingleEmailAsync() so that it will be recorded.")]
    Task<Exception> SendAsync(string body, string subject, string toAddress);
}
