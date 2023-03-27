using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthTemplateNET7.Shared.PublicModels;

//added
public class Email
{
    #region keys

    public int Id { get; set; }

    public int BatchId { get; set; }

    public Guid? RecipientId { get; set; }

    #endregion //keys

    [Column(TypeName = "smalldatetime")]
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "smalldatetime")]
    public DateTime? DateSent { get; set; }

    [Required, MaxLength(128)]
    public EmailSendResult EmailSendResult { get; set; }

    [Required, MaxLength(128)]
    public string ToAddress { get; set; }

    [MaxLength(64)]
    public string ToName { get; set; }

    #region ctors

    public Email() { }

    public Email(string toAddress)
    {
        ToAddress = toAddress;
    }

    #endregion
}
