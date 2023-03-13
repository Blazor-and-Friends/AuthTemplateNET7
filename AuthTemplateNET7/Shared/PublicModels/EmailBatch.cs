using AuthTemplateNET7.Shared.SharedServices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthTemplateNET7.Shared.PublicModels;
public class EmailBatch
{
    public int Id { get; set; }

    public bool AppendUnsubscribeLink { get; set; }

    public BatchStatus BatchStatus { get; set; }

    public string Body { get; set; }

    [Column(TypeName = "smalldatetime")]
    public DateTime? DateCompleted { get; set; }

    [Column(TypeName = "smalldatetime")]
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "smalldatetime")]
    public DateTime DeleteAfter { get; set; } = DateTime.UtcNow.AddYears(1);

    public bool DevOnly { get; set; }

    public List<Email> Emails { get; set; }

    public int ErrorsCount { get; set; }

    public Priority Priority { get; set; }

    public int SentEmailsCount { get; set; }

    [MaxLength(128)]
    public string Subject { get; set; }

    public int TotalEmailsCount { get; set; }

    #region UI

    [NotMapped]
    public bool Delete { get; set; }

    public string PauseBtnCss
    {
        get
        {
            if (BatchStatus == BatchStatus.InProgress) return "btn-danger";
            if (BatchStatus == BatchStatus.Paused) return "btn-success";
            return null;
        }
    }

    public string PauseBtnIconCss
    {
        get
        {
            if (BatchStatus == BatchStatus.InProgress) return "bi-pause-fill";
            if (BatchStatus == BatchStatus.Paused) return "bi-play-fill";
            return null;
        }
    }

    [NotMapped]
    public bool ShowingBody { get; set; } //

    [NotMapped]
    public bool ShowingEmails { get; set; } //

    #endregion //UI

    #region ctors

    public EmailBatch() { }

    public EmailBatch(string body, int deleteAfterDays, bool devOnly, Email sentEmail, string subject, Priority priority = Priority.High)
    {
        Subject = subject;
        Body = body.StartsWith("<") ? body : $"<div>{body}<div>";
        Emails = new(1) { sentEmail };
        DevOnly = devOnly;
        DeleteAfter = DateTime.UtcNow.AddDays(deleteAfterDays); //todo ScheduledController delete old email batches
        Priority = priority;
    }

    #endregion //ctors
}
