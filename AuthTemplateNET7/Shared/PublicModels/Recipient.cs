using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthTemplateNET7.Shared.PublicModels;

//added
public class Recipient
{
    [FormFactoryIgnore]
    public Guid Id { get; set; }

    [Required, MaxLength(128), EmailAddress]
    public string Address { get; set; }

    [Column(TypeName = "date")]
    [FormFactoryIgnore]
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;

    [MaxLength(32)]
    public string FirstName { get; set; }

    [MaxLength(32)]
    public string LastName { get; set; }

    [MaxLength(1024), DataType(DataType.MultilineText)]
    public string Notes { get; set; }

    [MaxLength(32)]
    [FormFactorySelectAll]
    public string Source { get; set; }

    [FormFactoryIgnore]
    public bool Unsubscribed { get; set; }

    /// <summary>
    /// For using third-party email address validation services
    /// </summary>
    [FormFactoryIgnore]
    public ValidationStatus ValidationStatus { get; set; }

    #region UI

    [NotMapped, FormFactoryIgnore]
    public bool Delete { get; set; }

    [NotMapped, FormFactoryIgnore]
    public bool Editing { get; set; }

    public string RowCss
    {
        get
        {
            if (ValidationStatus == ValidationStatus.Invalid) return "text-danger";
            if (Unsubscribed) return "text-warning";
            return null;
        }
    }

    [NotMapped, FormFactoryIgnore]
    public bool ShowingNotes { get; set; }

    #endregion
}
