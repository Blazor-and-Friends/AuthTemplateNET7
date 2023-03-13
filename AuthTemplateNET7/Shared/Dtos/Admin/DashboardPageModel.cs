using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthTemplateNET7.Shared.Dtos.Admin;
public class DashboardPageModel
{
    public int ContactMessagesCount { get; set; }

    public int FailedLogins { get; set; }

    [Obsolete("ef, json")]
    public DashboardPageModel() { }

    public DashboardPageModel(int contactMessagesCount, int failedLogins)
    {
        ContactMessagesCount = contactMessagesCount;
        FailedLogins = failedLogins;

    }
}
