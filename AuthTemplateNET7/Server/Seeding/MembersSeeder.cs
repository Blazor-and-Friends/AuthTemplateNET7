#if DEBUG
using AuthTemplateNET7.Shared.Dtos.Membership;

namespace AuthTemplateNET7.Server.Seeding;

//added
public class MembersSeeder
{
    private readonly DataContext dataContext;

    public MembersSeeder(DataContext dataContext)
    {
        this.dataContext = dataContext;
    }

    public List<Member> SeedMembersAndRoles()
    {
        string password = "helloDollyHowYeBe1!"; // same password for all for the login page
        string yourRealEmail = "franki@valli.com"; //todo for testing require email confirmation

        string[] displayNames = { "Franki Dev", "Barbara Admin", "Alice Customer" };
        string[] emails = { yourRealEmail, "barbara@eden.com", "alice@eve.com" };

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
#endif
