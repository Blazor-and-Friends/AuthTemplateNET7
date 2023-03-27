using AuthTemplateNET7.Server.Models;
using AuthTemplateNET7.Shared;
using AuthTemplateNET7.Shared.Dtos.Membership;
using AuthTemplateNET7.Shared.PublicModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AuthTemplateNET7.Server.Controllers;

//added
[Route("api/[controller]")]
[Authorize]
public class MembershipController : ControllerBase
{
    AuthRepo authRepo; //don't instantiate in constructor because there's two ctors, one with password hasher and one with just the data context
    private DataContext dataContext { get; set; }

    public MembershipController(DataContext dataContext)
    {
        this.dataContext = dataContext;
    }

    #region forgot password

    [AllowAnonymous]
    [HttpPost("create-reset-token")]
    public async Task<IActionResult> CreateResetToken([FromBody] ForgotPasswordDto model, [FromServices] EmailBatchRepo emailBatchRepo, [FromServices]LinkHelpers linkHelpers)
    {
        if(ModelState.IsValid)
        {
            authRepo = new(dataContext);

            string ipAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();

            (Guid resetToken, bool memberFound, bool serverError) = await authRepo.SetForgotPasswordTokenAsync(model, ipAddress);

            string okMessage = $"An email has been sent to {model.Email}. Click on the link in the email to reset your password. If you do not see the email, check your spam folder and be sure to add us to your safe senders list. You have 90 minutes to reset your password.";

            if (memberFound)
            {
                var emailSendResult = await emailBatchRepo.SendSingleEmailAsync(
                    body: generateResetLink(resetToken, linkHelpers),
                    devOnly: true,
                    subject: "Reset password",
                    model.Email,
                    false);

                serverError = emailSendResult == EmailSendResult.Error;

                if(emailSendResult == EmailSendResult.Pending)
                {
                    okMessage = $"An email will be sent to {model.Email} in the next few minutes. Click on the link in the email to reset your password. If you do not see the email, check your spam folder and be sure to add us to your safe senders list. You have 90 minutes to reset your password.";
                }
            }

            if (serverError) return StatusCode(500, "There was a problem sending a reset email. This issue has been logged. Contact the website owner.");

            return Ok(okMessage);
        }

        return BadRequest("Something's wrong with the form");
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordDto model, [FromServices] IHashPassword passwordHasher)
    {
        if (ModelState.IsValid)
        {
            authRepo = new(dataContext, passwordHasher);

            (bool success, bool serverError, string message, ClaimsPrincipal claimsPrincipal) = await authRepo.ResetPasswordAsync(model);

            if (success)
            {
                await HttpContext.SignInAsync(claimsPrincipal);
                return Ok();
            }

            if (serverError) return StatusCode(500, "There was an issue resetting your password. This issue has been logged. Contact the website owner.");

            return BadRequest(message);
        }

        return BadRequest("You have invalid fields. Check the form carefully, correct any mistakes");
    }

    string generateResetLink(Guid resetToken, LinkHelpers linkHelpers)
    {
        return linkHelpers.CreateLink($"membership/reset-password/{resetToken}", "Click here to reset your password");
    }

    #endregion

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
    public async Task<IActionResult> Login([FromServices]IHashPassword paswordHasher, [FromBody]LoginDto model)
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
    public async Task<IActionResult> Register([FromServices] IHashPassword passwordHasher, [FromBody]RegisterDto model)
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

    [HttpPost("verify-password")]
    public async Task<IActionResult> VerifyPassword([FromServices] IHashPassword paswordHasher, [FromBody] VerifyPasswordDto model)
    {
        authRepo = new(dataContext, paswordHasher);

        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var memberId);

        bool verified = await authRepo.VerifyPasswordAsync(memberId, model);

        return Ok(verified);
    }
}
