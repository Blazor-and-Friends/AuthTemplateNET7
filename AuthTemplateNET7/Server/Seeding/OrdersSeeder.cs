using AuthTemplateNET7.Shared.PublicModels;

namespace AuthTemplateNET7.Server.Seeding;

public class OrdersSeeder
{
    private readonly DataContext dataContext;
    private readonly List<Member> members;
    private readonly Random random;
    private readonly SeederServices seederServices;

    public OrdersSeeder(DataContext dataContext, List<Member> members, Random random, SeederServices seederServices)
    {
        this.dataContext = dataContext;
        this.random = random;
        this.seederServices = seederServices;
        this.members = members; //this should just be customers, but perhaps admin buys their own stuff, so does dev
    }

    public void Seed(List<Product> products, int minOrders, int maxOrders, int minItems, int maxItems)
    {
        int ordersCount = random.Next(minOrders, maxOrders);

        int itemsCount = 0;
        int productsCount = products.Count;
        Product selectedProduct;

        foreach (var m in members)
        {
            m.Orders = new(ordersCount);
            for (int i = 0; i < ordersCount; i++)
            {
                itemsCount = random.Next(minItems, maxItems);

                List<OrderItem> orderItems = new List<OrderItem>(itemsCount);

                for (int j = 0; j < itemsCount; j++)
                {
                    selectedProduct = products[random.Next(0, productsCount)];
                    OrderItem oi = new()
                    {
                        Description = selectedProduct.Name,
                        ImgUrl = selectedProduct.ImgUrl,
                        Quantity = random.Next(1, 5),
                        UnitPrice = selectedProduct.Price
                    };

                    orderItems.Add(oi);
                }

                Order order = new Order
                {
                    Date = seederServices.PastDate(30, 365),
                    Memo = seederServices.RandomString(15, 200),
                    OrderItems = orderItems,
                    OrderStatus = (OrderStatus)random.Next(0, 5),
                    Total = orderItems.Sum(m => m.LineTotal)
                };

                m.Orders.Add(order);
            }
        }
    }

    public void SeedSingle(Product product)
    {
        var member = members.First(m => m.DisplayName == "Franki Dev");

        OrderItem orderItem = new(product);

        Order order = new Order
        {
            Id = Guid.Parse("87593bab-75fc-40ca-a229-ade79e99a691"),
            MemberId = member.Id,
            OrderItems = new() { orderItem},
            OrderStatus = OrderStatus.Checkout,
            Total = product.Price
        };

        dataContext.Add(order);
    }
}
