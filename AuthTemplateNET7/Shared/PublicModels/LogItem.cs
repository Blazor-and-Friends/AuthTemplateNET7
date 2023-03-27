using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthTemplateNET7.Shared.SharedServices;

namespace AuthTemplateNET7.Shared.PublicModels;

//added
//todo DOCS why i have some models in the shared project and others in the server and use dtos
public class LogItem
{
    public int Id { get; set; }

    /// <summary>
    /// For setting the table- css on each item
    /// </summary>
    public BootstrapColor BootstrapColor { get; set; }

    [Column(TypeName = "smalldatetime")] //accurate to one minute, 4 bytes in db
    public DateTime DateTime { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "date")] //3 bytes
    public DateTime DeleteAfter { get; set; }

    [MaxLength(1024)]
    public string ErrorMessage { get; set; }

    [MaxLength(2048)]
    public string Message { get; set; }

    public string StackTraceOrJson { get; set; }

    #region getters/notmapped/ui

    public string CssClass
    {
        get
        {
            switch (BootstrapColor)
            {
                case BootstrapColor.Danger:
                    return "text-danger";
                case BootstrapColor.Info:
                    return "text-info";
                case BootstrapColor.Primary:
                    return "text-primary";
                case BootstrapColor.Secondary:
                    return "text-secondary";
                case BootstrapColor.Success:
                    return "text-success";
                case BootstrapColor.Warning:
                    return "text-warning";
                default:
                    return "";
            }
        }
    }

    /// <summary>
    /// Used for deleting.
    /// </summary>
    [NotMapped]
    public bool Selected { get; set; }

    [NotMapped]
    public bool ShowStackTrace { get; set; }

    #endregion

    #region ctors

    [Obsolete("Prefer using a parameterized ctor as the will ensure the log item is valid for storing in the db")]
    public LogItem() { }

    public LogItem(string message, int deleteAfterDays = 30, BootstrapColor bootstrapColor = BootstrapColor.Info)
    {
        Message = message.Shorten(2048);
        DeleteAfter = DateTime.AddDays(deleteAfterDays);
        BootstrapColor = bootstrapColor;
    }

    public LogItem(Exception e, string message = null) : this(message, 180, BootstrapColor.Danger)
    {
        while (e.InnerException != null) e = e.InnerException;
        ErrorMessage = e.Message.Shorten(1024);
        StackTraceOrJson = e.StackTrace;
    }

    #endregion //ctors
}
