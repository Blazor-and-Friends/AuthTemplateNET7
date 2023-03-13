using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthTemplateNET7.Shared.PublicModels;
public class Login
{
    #region keys

    public int Id { get; set; }

    public Guid? MemberId { get; set; }

    #endregion //keys

    #region props

    [Column(TypeName = "smalldatetime")]
    public DateTime DateTime { get; set; } = DateTime.UtcNow;

    [Required, MaxLength(15)]
    public string IpAddress { get; set; }

    public bool Success { get; set; }

    #endregion //props

    #region getters

    public string CssClass => Success ? "" : "text-danger";

    public string Result => Success ? "Success" : "Failed";

    #endregion //getters

    [Obsolete("ef, json only", true)]
    public Login() { }

    public Login(string ipAddress, Guid? memberId, bool success)
    {
        IpAddress = ipAddress;
        MemberId = memberId;
        Success = success;
    }
}
