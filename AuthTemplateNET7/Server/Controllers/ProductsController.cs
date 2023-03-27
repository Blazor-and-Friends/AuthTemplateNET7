using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthTemplateNET7.Server.Controllers;

//added

[Authorize]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly DataContext dataContext;

    public ProductsController(DataContext dataContext)
    {
        this.dataContext = dataContext;
    }

    [HttpGet("get-products")]
    public async Task<IActionResult> GetProducts()
    {
        var result = await dataContext.Products
            .AsNoTracking()
            .ToArrayAsync();

        return Ok(result);
    }
}
