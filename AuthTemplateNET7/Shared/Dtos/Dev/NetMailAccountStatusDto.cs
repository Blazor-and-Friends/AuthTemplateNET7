using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthTemplateNET7.Shared.Dtos.Dev;
public class NetMailAccountStatusDto
{
    [FormFactoryIgnore]
    public bool HaveAccountInfo { get; set; }

    [FormFactoryIgnore]
    public string StatusMessage { get; set; }

    [EmailAddress, Display(Description = "If you want to verify the account information is valid")]
    public string SendSampleEmailTo { get; set; }

    #region UI

    public string TableCss => HaveAccountInfo ? "text-success" : "text-warning";

    #endregion //UI
}
