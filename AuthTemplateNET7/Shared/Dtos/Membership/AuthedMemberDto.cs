namespace AuthTemplateNET7.Shared.Dtos.Membership;
public class AuthedMemberDto
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; }
    public string Email { get; set; }
    public string[] Roles { get; set; }
}
