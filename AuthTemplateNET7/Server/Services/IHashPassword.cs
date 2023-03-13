namespace AuthTemplateNET7.Server.Services;

//added
public interface IHashPassword
{
    (string hashedPassword, string salt) Hash(string password);

    bool VerifyPassword(string clearTextPassword, string hashedPassword, string salt);
}
