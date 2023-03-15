using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AuthTemplateNET7.Shared.PublicModels;

//added
public class ContactMessage
{
    [FormFactoryIgnore]
    public int Id { get; set; }

    [Column(TypeName = "smalldatetime")]
    [FormFactoryIgnore]
    public DateTime DateTime { get; set; } = DateTime.UtcNow;

    [MaxLength(128, ErrorMessage = "Must be 128 characters or less")]
    public string Subject { get; set; }

    [Required, MaxLength(2048, ErrorMessage = "Must be 2048 characters or less"), DataType(DataType.MultilineText)]
    public string Message { get; set; }

    [MaxLength(128, ErrorMessage = "Must be 128 characters or less"), EmailAddress(ErrorMessage = "This does not appear to be a valid email address"), Display(Description = "If you would like an email response, please provide your email address")]
    public string EmailAddress { get; set; }

    [DisplayName("Yes, add me to your email list")]
    public bool AddToEmailList { get; set; } = true;

    [MaxLength(32)]
    public string FirstName { get; set; }

    [MaxLength(32)]
    public string LastName { get; set; }

    string phone_;
    //MaxLength international numbers, extensions, see https://stackoverflow.com/a/68457894/2816057
    [MaxLength(20, ErrorMessage = "Phone can be no longer than 20 characters"), Phone(ErrorMessage = "This does not appear to be a phone number"), Display(Description = "If you would like a call or text, please provide your phone number")]
    public string Phone
    {
        get
        {
            if (string.IsNullOrWhiteSpace(phone_)) return null;

            var digits = Regex.Replace(phone_, "[^.0-9]", "");

            if (digits.Length != 10) return phone_;

            var areaCode = digits.Substring(0, 3);
            var exchange = digits.Substring(3, 3);
            var last = digits.Substring(6);

            return $"({areaCode}) {exchange}-{last}";
        }
        set
        {
            if(string.IsNullOrWhiteSpace(value)) phone_ = null;
            else phone_ = Regex.Replace(value, "[^.0-9]", "");
        }
    }

    [Display(Description = "Choose how you would like us to contact you, if applicable")]
    public PreferredContactMethod? PreferredContactMethod { get; set; }

    /// <summary>
    /// Indicates whether scheduled maintenance should not delete this message
    /// </summary>
    [FormFactoryIgnore]
    public bool SaveMessage { get; set; }

    #region getters/not mapped/admin

    [NotMapped, FormFactoryIgnore]
    public Disposition Disposition { get; set; }

    [NotMapped, FormFactoryIgnore]
    public string DispositionLabel { get; set; }

    public static string[] Labels => new[] { "Auto-delete", "Delete now", "Save" };

    public string Fullname
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(FirstName)
                && !string.IsNullOrWhiteSpace(LastName))
            {
                return FirstName + " " + LastName;
            }

            if (!string.IsNullOrWhiteSpace(FirstName))
            {
                return FirstName;
            }

            if (!string.IsNullOrWhiteSpace(LastName))
            {
                return LastName;
            }
            return "Not given";
        }
    }

    #endregion //getters/not mapped
}

public enum Disposition
{
    AutoDelete,
    DeleteNow,
    Save
}

public enum PreferredContactMethod : byte
{
    Any,
    Email,
    Text,
    Call
}
