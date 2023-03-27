using AuthTemplateNET7.Shared.PublicModels;

namespace AuthTemplateNET7.Server.Seeding;

//added
public class ContactMessagesSeeder
{
    private readonly DataContext dataContext;
    private readonly Random random;
    SeederServices seederServices;

    public ContactMessagesSeeder(DataContext dataContext, Random random, SeederServices seederServices)
    {
        this.dataContext = dataContext;
        this.random = random;
        this.seederServices = seederServices;
    }

    public void Seed(int min, int max)
    {
        int maxMessageLength = 1024; //when the message length isn't important, set it low


        int howMany = random.Next(min, max);

        List<ContactMessage> result = new(howMany);

        for (int i = 0; i < howMany; i++)
        {
            ContactMessage message = new()
            {
                DateTime = seederServices.PastDate(1, 30),
                Subject = seederServices.RandomString(5, 128),
                Message = seederServices.RandomString(50, maxMessageLength, 0),
                EmailAddress = seederServices.Email(),
                AddToEmailList = seederServices.RandomBool(),
                FirstName = seederServices.Firstname(),
                LastName = seederServices.Lastname(),
                Phone = seederServices.Phone(),
                PreferredContactMethod = (PreferredContactMethod)random.Next(4),
                SaveMessage = seederServices.RandomBool()
            };

            result.Add(message);
        }

        dataContext.AddRange(result);
    }
}