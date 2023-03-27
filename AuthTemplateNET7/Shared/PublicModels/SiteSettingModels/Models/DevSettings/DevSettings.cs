using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthTemplateNET7.Shared.PublicModels.SiteSettingModels.Models.DevSettings;
public class DevSettings
{
    public bool DeleteOldContactMessages { get; set; } = true;
    public bool DeleteOldLogIns { get; set; } = true;
    public bool DeleteOldLogItems { get; set; } = true;
    public bool DeleteOldEmailBatches { get; set; } = true;

    public bool LogMaintenanceActivity { get; set; } = true;

    public bool LogAllStripeWebhookActivity { get; set; } = true;

    public bool MaintenanceActivityOn { get; set; } = true;

    public bool NotifyWhenUncaughtExceptionThrown { get; set; } = true;

    /// <summary>
    /// The email account you want to get notifications sent to
    /// </summary>
    [FormFactorySelectAll]
    [EmailAddress(ErrorMessage = "This does not appear to be a valid email address")]
    [Display(Prompt = "Enter the email address you want to receive notifications here...")]
    public string SendNotificationsTo { get; set; }

    public static string Key = "DevSettings";
}
