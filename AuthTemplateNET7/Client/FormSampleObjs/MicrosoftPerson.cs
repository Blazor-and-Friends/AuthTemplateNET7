namespace AuthTemplateNET7.Client.FormSampleObjs;

public class MicrosoftPerson
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public static List<MicrosoftPerson> GetPeople()
    {
        var result = new MicrosoftPerson[]
        {
            new MicrosoftPerson
            {
                Id = Guid.Parse("e6d27d37-458e-4595-9103-c917563ff01c"),
                Name = "Scott Hanselman"
            },
            new MicrosoftPerson
            {
                Id = Guid.Parse("a47676dc-a98a-407a-aa31-67bc7f5eab61"),
                Name = "Jon Galloway"
            },
            new MicrosoftPerson
            {
                Id = Guid.Parse("f081b525-e2d2-421d-b468-1171563bf8cf"),
                Name = "Daniel Roth"
            },
            new MicrosoftPerson
            {
                Id = Guid.Parse("6752afec-062a-427b-93cc-3108ea66550d"),
                Name = "David Fowler"
            },
            new MicrosoftPerson
            {
                Id = Guid.Parse("c5c57240-534f-44a8-a46f-1decc9782807"),
                Name = "Damien Edwards"
            },
            new MicrosoftPerson
            {
                Id = Guid.Parse("c6fac274-71b3-4706-9a00-ed046dd1620a"),
                Name = "Steve Sanderson"
            },
            new MicrosoftPerson
            {
                Id = Guid.Parse("68fd0ebd-3a72-472b-af42-13c9c86c3f93"),
                Name = "Javier Calvarro"
            }
        };

        return result.OrderBy(m => m.Name).ToList();
    }
}
