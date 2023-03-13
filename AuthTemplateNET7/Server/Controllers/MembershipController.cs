using AuthTemplateNET7.Shared;
using AuthTemplateNET7.Shared.Dtos.Membership;
using AuthTemplateNET7.Shared.PublicModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AuthTemplateNET7.Server.Controllers;

//added
[Route("api/[controller]")]
[Authorize]
[ApiController]
public class MembershipController : ControllerBase
{
    AuthRepo authRepo; //don't instantiate in constructor because there's two ctors, one with password hasher and one with just the data context
    private readonly DataContext dataContext;

    public MembershipController(DataContext dataContext)
    {
        this.dataContext = dataContext;
    }

#if DEBUG
    [AllowAnonymous]
    [HttpGet("get-member-emails")]
    public async Task<IActionResult> GetMemberEmails()
    {
        //For the login page so that one can quickly switch between roles

        var members = await dataContext.Members.Include(m => m.Roles).ToArrayAsync();

        List<LoginNameAndEmailDebug> result = new(members.Length);

        foreach (var item in members)
        {
            result.Add(new LoginNameAndEmailDebug(item.DisplayName, item.Email, item.Roles.Select(m => m.Name).ToArray()));
        }

        return Ok(result);
    }
#endif

    [HttpGet("get-my-logins")]
    public async Task<IActionResult> GetMyLogins()
    {
        authRepo = new(dataContext);

        var emailAddress = User.FindFirstValue(ClaimTypes.Email);

        var result = await authRepo.GetMyLoginsAsync(emailAddress);

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromServices]IHashPassword paswordHasher, LoginDto model)
    {
        //todo rate limit this https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit?view=aspnetcore-7.0

        //sign out if member is signed in
        await HttpContext.SignOutAsync();

        authRepo = new(dataContext, paswordHasher);

        string ipAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();

        ClaimsPrincipal claimsPrincipal = await authRepo.LogInAsync(model, ipAddress);

        if(claimsPrincipal == null) return BadRequest();

        await HttpContext.SignInAsync(claimsPrincipal);

        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromServices] IHashPassword passwordHasher, RegisterDto model)
    {
        if (ModelState.IsValid) //to double-check that the password has an upper/lower case, etc
        {
            authRepo = new(dataContext, passwordHasher);

            (RegisterMemberResultDto registerMemberResultDto, ClaimsPrincipal claimsPrincipal) = await authRepo.RegisterAsync(model);

            if(registerMemberResultDto == null) //means we successfully added member
            {
                await HttpContext.SignInAsync(claimsPrincipal);
                return Ok();
            }

            if (registerMemberResultDto.RegistrationResult == RegistrationResult.ServerError) return StatusCode(500);

            return StatusCode(409, registerMemberResultDto);
        }
        return BadRequest();
    }
}
