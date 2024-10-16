using System.Security.Cryptography;
using System.Text;

namespace WebApp.DataAccess.Helpers;

public static class PasswordHasher
{
    private const int Iteration = 3;

    public static string ComputeHash(string password, string salt, string pepper, int iteration = Iteration)
    {
        if (iteration <= 0)
        {
            return password;
        }

        var passwordSaltPepper = $"{password}{salt}{pepper}";
        var byteValue = Encoding.UTF8.GetBytes(passwordSaltPepper);
        var byteHash = SHA256.HashData(byteValue);
        var hash = Convert.ToBase64String(byteHash);
        return ComputeHash(hash, salt, pepper, iteration - 1);
    }

    public static string GenerateSalt()
    {
        using var rng = RandomNumberGenerator.Create();
        var byteSalt = new byte[16];
        rng.GetBytes(byteSalt);
        var salt = Convert.ToBase64String(byteSalt);
        return salt;
    }
}