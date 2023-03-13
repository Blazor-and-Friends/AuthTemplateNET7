using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AuthTemplateNET7.Server.Models;

//added
[Index(nameof(Name), IsUnique = true)]
public class Role
{
    public int Id { get; set; }

    [Required, MaxLength(64)]
    public string Description { get; set; }

    public List<Member> Members { get; set; }

    [Required, MaxLength(16)]
    public string Name { get; set; }
}
