using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthTemplateNET7.Shared.Dtos.Membership;

//added
public class LoginDto
{
    [Required, MaxLength(128), EmailAddress(ErrorMessage = "This does not appear to be a valid email address")]
    public string Email { get; set; }

    [FormFactoryIgnore]
    [Required, PasswordPropertyText, DataType(DataType.Password)]
    public string Password { get; set; }
}
