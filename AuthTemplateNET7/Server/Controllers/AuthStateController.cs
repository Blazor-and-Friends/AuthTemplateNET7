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

        //todo Do i want to check the db everytime? What about a fired employee? How do i handle signing him out so that he can't mess with the data?
        if (User.Identity.IsAuthenticated)
        {
            //result = await dataContext.Members.Where(m => m.DisplayName == User.Identity.Name)
            //.Select(m => new AuthedMemberDto
            //{
            //    DisplayName = m.DisplayName,
            //    Email = m.Email,
            //})
            //.FirstOrDefaultAsync();

            result.DisplayName = User.FindFirstValue(ClaimTypes.Name);
            result.Email = User.FindFirstValue(ClaimTypes.Email);
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
