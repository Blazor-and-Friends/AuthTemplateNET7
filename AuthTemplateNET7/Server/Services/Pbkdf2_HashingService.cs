using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace AuthTemplateNET7.Server.Services;

//added
/// <summary>
/// Adapted from <see href="https://code-maze.com/csharp-hashing-salting-passwords-best-practices/"/>
/// </summary>
public class Pbkdf2_HashingService : IHashPassword
{
    HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
    const int iterations = 350_000;
    const int keySize = 64;

    public (string hashedPassword, string salt) Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(keySize);

        byte[] hashBytes = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            iterations,
            hashAlgorithm,
            keySize);

        return (Convert.ToHexString(hashBytes), Convert.ToHexString(salt));
    }

    public bool VerifyPassword(string clearTextPassword, string hashedPassword, string salt)
    {
        byte[] saltBytes = Convert.FromHexString(salt);

        byte[] hashToCompare = Rfc2898DeriveBytes.Pbkdf2(clearTextPassword, saltBytes, iterations, hashAlgorithm, keySize);

        return hashToCompare.SequenceEqual(Convert.FromHexString(hashedPassword));
    }
}
