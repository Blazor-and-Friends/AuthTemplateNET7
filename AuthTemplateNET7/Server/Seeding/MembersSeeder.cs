namespace AuthTemplateNET7.Server.Seeding;

//added
public class MembersSeeder
{
    private readonly DataContext dataContext;

    public MembersSeeder(DataContext dataContext)
    {
        this.dataContext = dataContext;
    }

    public List<Member> SeedMembersAndRoles(bool useMyRealEmailAddress)
    {
        string password = "helloDollyHowYeBe1!"; // same password for all for the login page

        string myRealEmail = null;

        if(useMyRealEmailAddress)
        {
            myRealEmail = Environment.GetEnvironmentVariable("MY_REAL_EMAIL_ADDRESS", EnvironmentVariableTarget.User);
        }

        string devEmailAddress = myRealEmail != null ? myRealEmail : "franki@valli.com"; //for testing you email service

        string[] guidStrs = { "e8b24c30-94f4-4c53-b3bc-835214e87111", "1d1ad76e-5c93-4c9f-ad80-6747fb962c2f", "e84707e0-d850-4d2b-aa59-4354d38a169f" };
        string[] displayNames = { "Franki Dev", "Barbara Admin", "Alice Customer" };
        string[] emails = { devEmailAddress, "barbara@eden.com", "alice@eve.com" };

        Pbkdf2_HashingService hashingService= new Pbkdf2_HashingService();

        int length = displayNames.Length;
        List<Member> result = new List<Member>(length);

        Role dev = new Role
        {
            Description = "For site dev pages",
            Name = "Dev"
        };

        Role admin = new Role
        {
            Description = "Hires/Fires employees, etc",
            Name = "Admin"
        };

        Role customer = new Role
        {
            Description = "The reason you have a website",
            Name = "Customer"
        };

        for (int i = 0; i < length; i++)
        {
            (string hashedPassword, string salt) = hashingService.Hash(password);
            Member member = new Member
            {
                Id = Guid.Parse(guidStrs[i]),
                DisplayName = displayNames[i],
                Email = emails[i],
                PasswordHash = hashedPassword,
                Salt = salt
            };

            if(i == 0) member.Roles = new List<Role> { dev, admin, customer };
            else if(i == 1) member.Roles = new List<Role> { admin, customer };
            else if(i == 2) member.Roles = new List<Role> { customer };

            result.Add(member);
        }

        dataContext.AddRange(result);
        return result;
    }
}