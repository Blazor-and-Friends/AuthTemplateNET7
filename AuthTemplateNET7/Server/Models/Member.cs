using AuthTemplateNET7.Shared.PublicModels;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthTemplateNET7.Server.Models;

//added
[Index(nameof(DisplayName), IsUnique = true)]
[Index(nameof(Email), IsUnique = true)]
public class Member
{
    public Guid Id { get; set; }

    [Required, MaxLength(32)]
    public string DisplayName { get; set; }

    [Required, MaxLength(128), EmailAddress]
    public string Email { get; set; }

    public List<Login> Logins { get; set; }

    [Required, MaxLength(128)]
    public string PasswordHash { get; set; }

    public List<Role> Roles { get; set; }

    [Required, MaxLength(128)]
    public string Salt { get; set; }
}
