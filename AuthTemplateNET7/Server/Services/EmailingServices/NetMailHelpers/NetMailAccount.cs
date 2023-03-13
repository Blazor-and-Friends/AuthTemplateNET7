namespace AuthTemplateNET7.Server.Services.EmailingServices.NetMailHelpers;

//added

public class NetMailAccount
{
    public string Address { get; set; }

    public bool EnableSsl { get; set; }

    public string Host { get; set; }

    public string Name { get; set; }

    public string Password { get; set; }

    public int Port { get; set; }

    public string Username { get; set; }
}
