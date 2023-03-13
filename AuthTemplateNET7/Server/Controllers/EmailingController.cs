using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthTemplateNET7.Server.Controllers;

//added

[Authorize(Roles = "Dev, Admin")]
[Route("api/[controller]")]
public class EmailingController : ControllerBase
{
    private readonly DataContext dataContext;

    public EmailingController(DataContext dataContext)
    {
        this.dataContext = dataContext;
    }

    [AllowAnonymous]
    [HttpGet("unsubscribe/{id}")]
    public async Task<IActionResult> Unsubscribe(Guid id)
    {
        RecipientsRepo repo = new(dataContext);
        string ip = Request.HttpContext.Connection.RemoteIpAddress.ToString();
        await repo.Unsubscribe(id, ip);

        return Ok(); //always return unsubscribed so that a user can't figure out if the email address exists/doesn't exist in db
    }
}
