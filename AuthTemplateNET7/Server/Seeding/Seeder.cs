namespace AuthTemplateNET7.Server.Seeding;

//added
public class Seeder
{
    private readonly DataContext dataContext;

    public Seeder(DataContext dataContext)
    {
        this.dataContext = dataContext;
    }

    public void Seed(string appKey)
    {
        //if there are already members in the db, we assume everything has been seeded.
        if (dataContext.Members.Any()) return;

        Random random = new();
        SeederServices seederServices = new(random);

        MembersSeeder membersSeeder = new(dataContext);

        //use these for whatever else you may need to seed.
        var members = membersSeeder.SeedMembersAndRoles(true);

        //ContactMessagesSeeder contactMessagesSeeder = new(dataContext, random, seederServices);
        //contactMessagesSeeder.Seed(3, 50);

        //var emailBatchSeeder = new EmailBatchSeeder(dataContext, random, seederServices);
        //emailBatchSeeder.Seed(10, 30);

        //LogItemsSeeder logItemsSeeder = new(dataContext, random, seederServices);
        //logItemsSeeder.Seed(10, 50);

        ProductsSeeder productsSeeder = new(dataContext, random);
        var products = productsSeeder.Seed(true, 5, 5);

        //OrdersSeeder ordersSeeder = new(dataContext, members, random, seederServices);

        //ordersSeeder.SeedSingle(products.First());

        //ordersSeeder.Seed(products, 3, 15, 1, 10);

        //RecipientsSeeder recipientSeeder = new(dataContext, random, seederServices);
        //recipientSeeder.Seed(5, 50);

        SiteSettingsSeeder siteSettingsSeeder = new(dataContext);
        siteSettingsSeeder.Seed();

        dataContext.SaveChanges();
    }

}