using AuthTemplateNET7.Shared.Dtos.Public;
using AuthTemplateNET7.Shared.PublicModels;
using AuthTemplateNET7.Shared.SharedServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthTemplateNET7.Server.Controllers;

//added

[Route("api/[controller]")]
public class InteractionController : ControllerBase
{
    InteractionRepo interactionRepo;

    public InteractionController(DataContext dataContext)
    {
        interactionRepo = new InteractionRepo(dataContext);
    }

    [HttpPost("join-email-list")]
    public async Task<IActionResult> JoinEmailList([FromBody] JoinEmailListDto model)
    {
        if (ModelState.IsValid)
        {
            string ip = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            var success = await interactionRepo.MaybeAddToEmailList(model, ip);

            if (success) return Ok();

            return StatusCode(500, "There was an issue adding you to our email list. This issue has been logged and will be addressed, at which time we will add you to our email list.");
        }

        //if they're making it past client model validation, there's hanky panky going on. Fail silently
        return Ok();
    }

    [HttpPost("post-message")]
    public async Task<IActionResult> PostMessage([FromBody]ContactMessage model, [FromServices]LinkHelpers linkHelpers)
    {
        if (ModelState.IsValid)
        {
            var success = await interactionRepo.CreateContactMessageAsync(model, linkHelpers);

            if (success) return Ok();

            return StatusCode(500, "There was a problem saving your message. This has been logged and will be addressed soon.");
        }

        //if they're getting past client-side validation, there's some hanky-panky going on. Don't bother with telling them what to correct
        return BadRequest("You have some invalid fields. Please review the form carefully and correct any mistakes.");
    }
}
