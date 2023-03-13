using System.Security.Claims;

namespace AuthTemplateNET7.Shared.SharedServices;

//added
public class SharedAuthServices
{
    public ClaimsPrincipal CreateClaimsPrincipal(string authenticationType, string email, IEnumerable<string> roles, string username)
    {
        List<Claim> claims = new List<Claim>(6);

        claims.Add(new Claim(ClaimTypes.Name, username));
        claims.Add(new Claim(ClaimTypes.Email, email));

        foreach (var item in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, item));
        }

        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, authenticationType);

        return new ClaimsPrincipal(claimsIdentity);
    }
}
