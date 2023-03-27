using AuthTemplateNET7.Shared;
using AuthTemplateNET7.Shared.PublicModels;

namespace AuthTemplateNET7.Server.Seeding;

//added
public class EmailBatchSeeder
{
    private readonly DataContext dataContext;
    private readonly Random random;
    private readonly SeederServices seederServices;

    public EmailBatchSeeder(DataContext dataContext, Random random, SeederServices seederServices)
    {
        this.dataContext = dataContext;
        this.random = random;
        this.seederServices = seederServices;
    }

    public void Seed(int min, int max)
    {
        int howMany = random.Next(min, max);

        List<Batch> result = new(howMany);
        DateTime utcNow = DateTime.UtcNow;

        for (int i = 0; i < howMany; i++)
        {
            BatchStatus batchStatus = (BatchStatus)random.Next(3);

            int daysAgo = random.Next(10, 30);

            DateTime? dateCompleted = null;
            if(batchStatus == BatchStatus.Complete)
            {
                dateCompleted = utcNow.AddDays(-daysAgo + random.Next(0, daysAgo));
            }

            DateTime dateCreated = utcNow.AddDays(-daysAgo);

            DateTime deleteAfter = utcNow.AddDays(random.Next(365));

            List<Email> emails = getEmails(random, dateCreated, dateCompleted);

            Priority priority = (Priority)random.Next(3);

            Batch batch = new Batch
            {
                BatchStatus = batchStatus,
                Body = getBody(),
                DateCompleted = dateCompleted,
                DateCreated = dateCreated,
                DeleteAfter = deleteAfter,
                DevOnly = seederServices.RandomBool(),
                Emails = emails,
                ErrorsCount = emails.Count(m => m.EmailSendResult == EmailSendResult.Error),
                Priority = priority,
                SentEmailsCount = emails.Count(m => m.EmailSendResult == EmailSendResult.Success),
                Subject = $"Hello Dolly! {random.Next(1000)}",
                TotalEmailsCount = emails.Count
            };

            result.Add(batch);
        }

        dataContext.AddRange(result);
    }

    string getBody()
    {
        return $"<h1>Hello Dolly!</h1><p>{seederServices.RandomString(50, 1000, 0)}</p>";
    }

    List<Email> getEmails(Random random, DateTime dateAdded, DateTime? dateSent)
    {
        int howMany = random.Next(1, 10);

        List<Email> result = new(howMany);

        for (int i = 0;i < howMany;i++)
        {
            EmailSendResult sendResult;

            if (!dateSent.HasValue) sendResult = EmailSendResult.Pending;
            else
            {
                sendResult = (EmailSendResult)random.Next(1, 3);
            }

            Email email = new Email
            {
                DateAdded = dateAdded,
                DateSent = dateSent,
                EmailSendResult = sendResult,
                ToAddress = seederServices.Email(0),
                ToName = seederServices.Firstname(),
            };

            result.Add(email);
        }

        return result;
    }
}