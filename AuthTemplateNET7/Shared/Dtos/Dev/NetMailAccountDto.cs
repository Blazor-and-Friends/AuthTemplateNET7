using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthTemplateNET7.Shared.Dtos.Dev;

//added

public class NetMailAccountDto : IValidatableObject
{
    [EmailAddress, Display(Description = "The address the website will send emails from", Prompt = "E.G. no-reply@customersDomain.com")]
    public string Address { get; set; }

    [Display(Description = "The name you want to appear in the 'from' field in recipients' inboxes")]
    public string Name { get; set; }

    public string Username { get; set; }

    [DataType(DataType.Password)]
    public string Password { get; set; }

    public bool EnableSsl { get; set; }

    [Display(Prompt = "E.G. smtp.google.com")]
    public string Host { get; set; }

    public int Port { get; set; }

    #region UI

    [FormFactoryIgnore]
    public bool AccountIsStored { get; set; }

    public bool DeleteAccount { get; set; }

    [EmailAddress, Display(Description = "If you want to verify the account information is valid")]
    public string SendSampleEmailTo { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        List<ValidationResult> result = new List<ValidationResult>();

        //if any fields are filled out...
        if(!string.IsNullOrWhiteSpace(Address)
            || !string.IsNullOrWhiteSpace(Host)
            || !string.IsNullOrWhiteSpace(Name)
            || !string.IsNullOrWhiteSpace(Password)
            || Port > 0
            || !string.IsNullOrWhiteSpace(Username)
            )
        {
            //they all need to be filled out
            if(string.IsNullOrWhiteSpace(Address)
                || string.IsNullOrWhiteSpace(Host)
                || string.IsNullOrWhiteSpace(Name)
                || string.IsNullOrWhiteSpace(Password)
                || Port < 1
                || string.IsNullOrWhiteSpace(Username)
                )
            {
                result.Add(new ValidationResult("All these fields need to be filled out for System.Net.Mail to work", new[] { "Address", "Host", "Name", "Password", "Port", "Username" }));
            }
        }

        return result;
    }

    #endregion //UI
}
