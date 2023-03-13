using AuthTemplateNET7.Shared;
using AuthTemplateNET7.Shared.Dtos.Membership;
using AuthTemplateNET7.Shared.PublicModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AuthTemplateNET7.Server.Data;

public class AuthRepo
{
    private readonly DataContext dataContext;
    private readonly IHashPassword passwordHasher;

    const string ADMIN_ROLE_NAME = "Admin";
    const string CUSTOMER_ROLE_NAME = "Customer";

    public AuthRepo(DataContext dataContext)
    {
        this.dataContext = dataContext;
    }
    public AuthRepo(DataContext dataContext, IHashPassword passwordHasher) : this(dataContext)
    {
        this.passwordHasher = passwordHasher;
    }

    public async Task<Login[]> GetMyLoginsAsync(string emailAddress)
    {
        var memberId = await dataContext.Members.Where(m => m.Email == emailAddress).Select(m => m.Id).FirstOrDefaultAsync();
        return await dataContext.Logins.Where(m => m.MemberId == memberId).OrderByDescending(m => m.DateTime).ToArrayAsync();
    }

    #region Log in

    public async Task<ClaimsPrincipal> LogInAsync(LoginDto model, string ipAddress)
    {
        bool success = true;
        Guid? memberId = null;
        var memberAnon = await dataContext.Members.AsNoTracking().Select(m => new
        {
            m.Id,
            m.DisplayName,
            m.Email,
            m.PasswordHash,
            m.Roles,
            m.Salt,
        }).FirstOrDefaultAsync(m => m.Email == model.Email);

        if (memberAnon == null) success = false;

        if (success)
        {
            success = passwordHasher.VerifyPassword(model.Password, memberAnon.PasswordHash, memberAnon.Salt);
            memberId = memberAnon.Id;
        }

        await createLogin(ipAddress: ipAddress,
            memberId: memberId,
            success: success,
            suppliedEmailAddress: model.Email);

        if (success) return new SharedAuthServices().CreateClaimsPrincipal(
            authenticationType: "serverAuth",
            email: model.Email,
            roles: memberAnon.Roles.Select(m => m.Name).ToArray(),
            username: memberAnon.DisplayName);

        return null;
    }

    Task createLogin(string ipAddress, Guid? memberId, bool success, string suppliedEmailAddress)
    {
        Login login = new(ipAddress, memberId, success);
        dataContext.Add(login);
        return dataContext.TrySaveAsync($"Could not create Login for Member.Id {memberId.ToString()} with supplied email {suppliedEmailAddress}");
    }

    #endregion //log in

    #region Register
    public async Task<(RegisterMemberResultDto registerMemberResultDto, ClaimsPrincipal claimsPrincipal)> RegisterAsync(RegisterDto model)
    {
        RegisterMemberResultDto result = await memberExists(model);
        if (result != null) return (result, null);

        (string hashedPassword, string hashedSalt) = passwordHasher.Hash(model.Password);

        var customerRole = await dataContext.Roles.Where(m => m.Name == CUSTOMER_ROLE_NAME).FirstOrDefaultAsync();

        Member member = new()
        {
            DisplayName = model.DisplayName,
            Email = model.EmailAddress,
            PasswordHash = hashedPassword,
            Roles = new List<Role> { customerRole },
            Salt = hashedSalt
        };

        dataContext.Add(member);

        var rows = await dataContext.TrySaveAsync($"Could not register a member with email address {model.EmailAddress}.");

        if (rows == 0) //there was a db issue
        {
            RegisterMemberResultDto registerMemberResultDto = new()
            {
                MessageToUser = "There was a server error. This has been logged and will be addressed.",
                RegistrationResult = RegistrationResult.ServerError
            };

            return (registerMemberResultDto, null);
        }

        ClaimsPrincipal cp = new SharedAuthServices().CreateClaimsPrincipal(
            authenticationType: "serverAuth",
            email: model.EmailAddress,
            new string[] { CUSTOMER_ROLE_NAME },
            username: model.DisplayName);

        return (null, cp);
    }

    async Task<RegisterMemberResultDto> memberExists(RegisterDto model)
    {
        var memberAnon = await dataContext.Members.Where(m => m.DisplayName == model.DisplayName || m.Email == model.EmailAddress)
            .Select(m => new
            {
                m.Email,
                m.DisplayName
            })
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (memberAnon == null) return null;

        RegisterMemberResultDto result = new();

        string message;

        if (memberAnon.Email == model.EmailAddress && memberAnon.DisplayName == model.DisplayName)
        {
            message = "This member already exists in our system. Did you forget your password?";
            result.RegistrationResult = RegistrationResult.EmailAndDisplayExist;
        }
        else if (memberAnon.Email == model.EmailAddress)
        {
            message = "This member already exists in our system. Did you forget your password?";
            result.RegistrationResult = RegistrationResult.EmailAddressExists;
        }
        else if (memberAnon.DisplayName == model.DisplayName)
        {
            message = "The display name is already taken. Here are some suggested alternatives:";
            result.RegistrationResult = RegistrationResult.DisplayNameExists;
            result.SuggestedDisplayNames = await createSuggestedDisplayNames(model.DisplayName);
        }
        else message = string.Empty;

        result.MessageToUser = message;

        return result;
    }

    async Task<List<string>> createSuggestedDisplayNames(string duplicateDisplayName)
    {
        var existingDisplayNamesDict = await dataContext.Members.Where(m => m.DisplayName.Contains(duplicateDisplayName)).AsNoTracking().ToDictionaryAsync(k => k.DisplayName, v => false);

        List<string> result = new List<string>();

        Random random = new Random();

        while (result.Count < 4)
        {
            string suggestion = duplicateDisplayName + random.Next(1, 10_000);

            if (existingDisplayNamesDict.ContainsKey(suggestion)) continue;

            result.Add(suggestion);
        }

        return result;
    }

    #endregion //register
}
