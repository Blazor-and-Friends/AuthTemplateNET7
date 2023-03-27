using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthTemplateNET7.Shared.Dtos.Membership;
public class VerifyPasswordDto
{
    [Required, DataType(DataType.Password)]
    public string Password { get; set; }
}
