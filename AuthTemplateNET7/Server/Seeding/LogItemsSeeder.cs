using AuthTemplateNET7.Shared;
using AuthTemplateNET7.Shared.PublicModels;

namespace AuthTemplateNET7.Server.Seeding;

public class LogItemsSeeder
{
    private readonly DataContext dataContext;
    private readonly Random random;
    private readonly SeederServices seederServices;

    public LogItemsSeeder(DataContext dataContext, Random random, SeederServices seederServices)
    {
        this.dataContext = dataContext;
        this.random = random;
        this.seederServices = seederServices;
    }

    public void Seed(int min, int max)
    {
        int howMany = random.Next(min, max);

        List<LogItem> result = new(howMany);

        BootstrapColor bootstrapColor;

        for (int i = 0; i < howMany; i++)
        {
            bootstrapColor = (BootstrapColor)random.Next(7);

            LogItem logItem;

            if (bootstrapColor == BootstrapColor.Danger)
            {
                int dividend = 42;
                int divisor = 0;

                try
                {
                    var quotient = dividend / divisor;
                    logItem = new("dummy");
                }
                catch (Exception e)
                {
                    logItem = new LogItem(e, seederServices.RandomString(15, 500));
                    var st = e.StackTrace + "\r\n";
                    int lines = random.Next(15);
                    for (int j = 0; j < lines; j++)
                    {
                        logItem.StackTrace += st;
                    }
                }
            }
            else
            {
#pragma warning disable CS0618
                logItem = new()
                {
                    BootstrapColor = bootstrapColor,
                    DateTime = seederServices.PastDate(1, 60),
                    DeleteAfter = seederServices.FutureDate(1, 180),
                    Message = seederServices.RandomString(15, 2048)
                };
#pragma warning restore CS0618
            }

            result.Add(logItem);
        }

        dataContext.AddRange(result);
    }
}
