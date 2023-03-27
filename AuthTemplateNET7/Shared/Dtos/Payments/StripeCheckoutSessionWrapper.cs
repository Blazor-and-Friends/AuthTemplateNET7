using AuthTemplateNET7.Shared.PublicModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthTemplateNET7.Shared.Dtos.Payments;
public class StripeCheckoutSessionWrapper
{
    public Order Order { get; set; }

    public string SessionUrl { get; set; }

    [Obsolete("json only")]
    public StripeCheckoutSessionWrapper() { }

    public StripeCheckoutSessionWrapper(Order order, string sessionUrl)
    {
        Order = order;
        SessionUrl = sessionUrl;
    }
}
