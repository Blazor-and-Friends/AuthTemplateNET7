using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthTemplateNET7.Shared.Dtos.Dev;
public class StripeAccountStatusDto
{
    public string LiveKeysStatus { get; set; }
    public string TestKeysStatus { get; set; }
    public string TestModeStatus { get; set; }
}
