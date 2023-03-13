namespace AuthTemplateNET7.Client.DeleteMeFf;

public class PersonWithInt
{
    public int Id { get; set; }

    public string Name { get; set; }

    public static List<PersonWithInt> GetPeople()
    {
        return new List<PersonWithInt>
        {
            new PersonWithInt { Id = 1, Name= "Anne" },
            new PersonWithInt { Id = 2, Name= "Betty" },
            new PersonWithInt { Id = 3, Name= "Charlie" },
        };
    }
}
