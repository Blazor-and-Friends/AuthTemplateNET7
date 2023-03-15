using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthTemplateNET7.Shared.Dtos.Dev;
public class Stripe
{
    public string Public { get; set; }
    public string Secret { get; set; }
    public string TestPublic { get; set; }
    public string TestSecret { get; set; }
}
