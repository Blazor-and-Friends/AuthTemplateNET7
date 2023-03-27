using AuthTemplateNET7.Shared;
using AuthTemplateNET7.Shared.Dtos.Membership;
using AuthTemplateNET7.Shared.PublicModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AuthTemplateNET7.Server.Data;

public class AuthRepo : RepoBase
{
    private readonly IHashPassword passwordHasher;

    const string ADMIN_ROLE_NAME = "Admin";
    const string CUSTOMER_ROLE_NAME = "Customer";

    public AuthRepo(DataContext dataContext) : base(dataContext) { }

    public AuthRepo(DataContext dataContext, IHashPassword passwordHasher) : this(dataContext)
    {
        this.passwordHasher = passwordHasher;
    }

    #region forgot password

    public async Task<(Guid resetToken, bool memberFound, bool serverError)> SetForgotPasswordTokenAsync(ForgotPasswordDto model, string ip)
    {
        var member = await dataContext.Members.FirstOrDefaultAsync(m => m.Email == model.Email);

        if(member == null)
        {
            await logMemberNotFoundForPasswordReset(model.Email, ip);
            return (Guid.Empty, false, false);
        }

        member.ForgotPasswordToken = Guid.NewGuid();
        member.ForgotPasswordExpiration = DateTime.UtcNow.AddMinutes(90);

        _ = dataContext.Update(member);

        var rows = await dataContext.TrySaveAsync($"Could not create a reset token for Member.Id {member.Id.ToString()} with Member.Email {member.Email}");

        if(rows > 0) return (member.ForgotPasswordToken.Value, true, false);
        return (Guid.Empty, false, true);
    }

    public async Task<(bool success, bool serverError, string message, ClaimsPrincipal claimsPrincipal)> ResetPasswordAsync(ResetPasswordDto model)
    {
        if(model.ResetToken == Guid.Empty) return (false, false, "Bad request", null);

        var member = await dataContext.Members.Include(m => m.Roles).FirstOrDefaultAsync(m => m.ForgotPasswordToken == model.ResetToken);

        if(member == null) return (false, false, "Bad request", null);

        if(member.ForgotPasswordExpiration.HasValue && member.ForgotPasswordExpiration.Value < DateTime.UtcNow)
        {
            return (false, false, "Your reset token has expired.", null);
        }

        member.ForgotPasswordExpiration = null;
        member.ForgotPasswordToken = null;

        (string hashedPassword, string hashedSalt) = passwordHasher.Hash(model.Password);

        member.PasswordHash = hashedPassword;
        member.Salt = hashedSalt;

        _ = dataContext.Update(member);

        var rows = await dataContext.TrySaveAsync($"Could not reset password for Member.Id {member.Id} with Member.Email {member.Email}");

        if(rows > 0)
        {
            ClaimsPrincipal cp = new SharedAuthServices().CreateClaimsPrincipal(
            authenticationType: "serverAuth",
            email: member.Email,
            id: member.Id,
            roles: member.Roles.Select(m => m.Name).ToArray(),
            username: member.DisplayName);

            return (true, false, null, cp);
        }

        return (false, true, null, null);
    }

    Task logMemberNotFoundForPasswordReset(string email, string ip)
    {
        LogItem logItem = new($"Someone tried to reset their password with email {email} from <a href='href='https://www.bing.com/search?q={ip}' target='_blank'>{ip}</a>",
                360,
                BootstrapColor.Danger);

        _ = dataContext.Add(logItem);

        return dataContext.TrySaveAsync($"Could not log someone trying to reset password with email {email}");
    }

    #endregion //forgot password

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
            id: memberId.Value,
            roles: memberAnon.Roles.Select(m => m.Name).ToArray(),
            username: memberAnon.DisplayName);

        return null;
    }

    Task createLogin(string ipAddress, Guid? memberId, bool success, string suppliedEmailAddress)
    {
        Login login = new(ipAddress, memberId, success);
        _ = dataContext.Add(login);
        return dataContext.TrySaveAsync($"Could not create Login for Member.Id {memberId.ToString()} with supplied email {suppliedEmailAddress}");
    }

    #endregion //log in

    #region Register
    public async Task<(RegisterMemberResultDto registerMemberResultDto, ClaimsPrincipal claimsPrincipal)> RegisterAsync(RegisterDto model)
    {
        RegisterMemberResultDto result = await memberExists(model);
        if (result != null) return (result, null);

        HtmlSanitizerService sanitizerService = new();
        sanitizerService.SanitizeRegisterDto(model);

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

        _ = dataContext.Add(member);

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
            id: member.Id,
            roles: new string[] { CUSTOMER_ROLE_NAME },
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

    public async Task<bool> VerifyPasswordAsync(Guid memberId, VerifyPasswordDto model)
    {
        var member = await dataContext.Members
            .Where(m => m.Id == memberId)
            .FirstOrDefaultAsync();

        if (member == null)
        {
            LogItem logItem = new($"Someone tried to verify a password. MemberId passed in: {memberId}", deleteAfterDays: 90, bootstrapColor: BootstrapColor.Danger);
            return false;
        }

        var success = passwordHasher.VerifyPassword(model.Password, member.PasswordHash, member.Salt);

        if (success)
        {
            member.PasswordVerifitedExpiration = DateTime.UtcNow.AddMinutes(2);
            dataContext.Update(member);
            await dataContext.TrySaveAsync();
        }
        else
        {
            LogItem logItem = new($"Password verification failed for Member.Id {memberId} with Member.DisplayName {member.DisplayName}", deleteAfterDays: 90, bootstrapColor: BootstrapColor.Warning);

            await CreateLogItemAsync(logItem);
        }

        return success;
    }
}
