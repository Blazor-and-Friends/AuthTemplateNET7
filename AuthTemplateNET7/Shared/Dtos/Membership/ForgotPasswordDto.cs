using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthTemplateNET7.Shared.Dtos.Membership;
public class ForgotPasswordDto
{
    [Required, EmailAddress(ErrorMessage = "This does not appear to be a valid email address"), Display(Prompt = "Enter your email address here...")]
    public string Email { get; set; }
}
