using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthTemplateNET7.Shared.Dtos.Dev;

//added

public class NetMailAccountDto //: IValidatableObject
{
    [Required, EmailAddress, Display(Description = "The address the website will send emails from", Prompt = "E.G. no-reply@customersDomain.com")]
    public string Address { get; set; }

    [Required, Display(Description = "The name you want to appear in the 'from' field in recipients' inboxes", Prompt = "E.G. Blazor and Friends")]
    public string Name { get; set; }

    [Required]
    public string Username { get; set; }

    [Required, DataType(DataType.Password)]
    public string Password { get; set; }

    public bool EnableSsl { get; set; }

    [Required, Display(Prompt = "E.G. smtp.google.com")]
    public string Host { get; set; }

    public int Port { get; set; }

    [EmailAddress, Display(Description = "If you want to verify the account information is valid")]
    public string SendSampleEmailTo { get; set; }
}
