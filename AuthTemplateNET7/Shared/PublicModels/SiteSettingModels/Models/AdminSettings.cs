using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthTemplateNET7.Shared.PublicModels.SiteSettingModels.Models;
public class AdminSettings
{
    [EmailAddress(ErrorMessage = "This does not appear to be a valid email address")]
    [Display(Description = "The email address you want to receive notifications from the website. If this is left blank, you will not recieve notifications.", Prompt = "Enter email address here...")]
    [FormFactorySelectAll]
    public string SendNotificationsTo { get; set; }

    public bool NotifyMeWhenThereIsANewContactMessage { get; set; } = true;

    [Range(7, 90, ErrorMessage = "Must be between 7 and 90 days")]
    public int DeleteContactMessagesOlderThan { get; set; } = 30;

    [Range(7, 90, ErrorMessage = "Must be between 7 and 90 days")]
    public int DeleteLoginsOlderThan { get; set; } = 30;

    public static string Key = "AdminSettings";
}
