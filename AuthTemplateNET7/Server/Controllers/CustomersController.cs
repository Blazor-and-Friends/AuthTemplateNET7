using AuthTemplateNET7.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthTemplateNET7.Server.Controllers;

//added

[Authorize(Roles = "Customer")]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    CustomersRepo customersRepo;

    public CustomersController(DataContext dataContext)
    {
        customersRepo = new(dataContext);
    }

    [HttpGet("get-orders")]
    public async Task<IActionResult> GetOrders()
    {
        return Ok(await customersRepo.GetOrdersAsync(getMemberId()));
    }
    Guid getMemberId()
    {
        Guid result;

        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out result);

        return result;
    }
}
