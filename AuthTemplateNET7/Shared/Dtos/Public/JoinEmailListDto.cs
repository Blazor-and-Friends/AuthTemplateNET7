using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthTemplateNET7.Shared.Dtos.Public;
public class JoinEmailListDto
{
    [Required(ErrorMessage = "Enter your email address please"),
        EmailAddress(ErrorMessage = "This does not appear to be a valid email address"),
        MaxLength(128, ErrorMessage = "Must be less than 128 characters")]
    public string EmailAddress { get; set; }

    /// <summary>
    /// Honey pot to filter bots
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Honey pot to filter bots
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// If the email address is filled out too quickly, it's probably a bot
    /// </summary>
    public int SecondsToSubmit { get; set; }

    public string Source { get; set; }
}
