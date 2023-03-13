namespace AuthTemplateNET7.Client.FormSampleObjs;

public class ProgrammingLanguage
{
    public int Id { get; set; }

    public string Name { get; set; }


    public static List<ProgrammingLanguage> Get()
    {
        string[] ls = new string[] { "C#", "CSS", "HTML", "SQL" };

        List<ProgrammingLanguage> result = new List<ProgrammingLanguage>(ls.Length);

        for (int i = 0; i < ls.Length; i++)
        {
            result.Add(new ProgrammingLanguage
            {
                Id = i + 1,
                Name = ls[i],
            });
        }

        return result;
    }
}
