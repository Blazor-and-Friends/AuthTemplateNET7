using AuthTemplateNET7.Shared.Dtos.Membership;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthTemplateNET7.Server.Controllers;

//added
[Route("api/[controller]")]
[ApiController]
public class AuthStateController : ControllerBase
{
    //keeping these endpoints separate because they don't need the DataContext

    [HttpGet("get-auth-state")]
    public IActionResult GetAuthState()
    {
        AuthedMemberDto result = new();

        if (User.Identity.IsAuthenticated)
        {
            Guid memberId;
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out memberId);

            result.DisplayName = User.FindFirstValue(ClaimTypes.Name);
            result.Email = User.FindFirstValue(ClaimTypes.Email);
            result.Id = memberId;
            result.Roles = User.FindAll(ClaimTypes.Role).Select(m => m.Value).ToArray();
        }

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("logout")]
    public async Task<IActionResult> LogOut()
    {
        await HttpContext.SignOutAsync();
        return Ok();
    }
}
