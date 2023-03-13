#if DEBUG

namespace AuthTemplateNET7.Shared.Dtos.Membership;
public class LoginNameAndEmailDebug
{
    public string DisplayName { get; set; }
    public string Email { get; set; }

    public string[] Roles { get; set; }

    [Obsolete("ef, json only", true)]
    public LoginNameAndEmailDebug() { }

    public LoginNameAndEmailDebug(string displayName, string email, string[] roles)
    {
        DisplayName = displayName;
        Email = email;
        Roles = roles;
    }
}
#endif