using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthTemplateNET7.Shared.Dtos.Membership;
public class RegisterMemberResultDto
{
    /// <summary>
    /// Message to the user who is trying to register
    /// </summary>
    public string MessageToUser { get; set; }

    public RegistrationResult RegistrationResult { get; set; }

    /// <summary>
    /// Suggest a list of display names to the user
    /// </summary>
    public List<string> SuggestedDisplayNames { get; set; }
}
