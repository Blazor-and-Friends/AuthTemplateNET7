using AuthTemplateNET7.Shared.PublicModels;
using Microsoft.EntityFrameworkCore;

namespace AuthTemplateNET7.Server.Data;

public class CustomersRepo
{
    private readonly DataContext dataContext;

    public CustomersRepo(DataContext dataContext)
    {
        this.dataContext = dataContext;
    }

    public Task<Order[]> GetOrdersAsync(Guid memberId)
    {
        return dataContext.Orders
            .Where(m => m.MemberId == memberId)
            .OrderByDescending(m => m.Date)
            .Include(m => m.OrderItems)
            .ToArrayAsync();
    }
}
