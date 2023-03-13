#if DEBUG
using AuthTemplateNET7.Shared;
using AuthTemplateNET7.Shared.PublicModels;

namespace AuthTemplateNET7.Server.Seeding;

//added
public class RecipientsSeeder
{
    private readonly DataContext dataContext;
    Random random;
    SeederServices seederServices;

    public RecipientsSeeder(DataContext dataContext, Random random, SeederServices seederServices)
    {
        this.dataContext = dataContext;
        this.random = random;
        this.seederServices = seederServices;
    }

    public void Seed(int min, int max)
    {
        int howMany = random.Next(min, max);

        List<Recipient> recipients = new(howMany);

        for (int i = 0; i < howMany; i++)
        {
            Recipient recipient = new()
            {
                Address = seederServices.Email(0),
                DateAdded = seederServices.PastDate(3, 30),
                FirstName = seederServices.Firstname(),
                LastName = seederServices.Lastname(),
                Notes = seederServices.RandomString(5, 1024),
                Source = seederServices.RandomString(5, 32),
                Unsubscribed = seederServices.RandomBool(),
                ValidationStatus = (ValidationStatus)random.Next(3)
            };

            recipients.Add(recipient);
        }

        dataContext.AddRange(recipients);
    }
}

#endif
