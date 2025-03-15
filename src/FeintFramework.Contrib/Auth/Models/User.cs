using System;
using System.Security.Cryptography;
using FeintFramework.Contrib.Auth.Models;
using LinqToDB.Mapping;

namespace FeintFramework.Contrib.Auth;


[Table(Name = "auth_user")]
public partial class User : AbstractUser
{
   
    protected virtual string PasswordHash(string password)
    {
        const int iterations = 260000;
        const int saltSize = 16; 
        const int hashSize = 32; 
        byte[] saltBytes = new byte[saltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        string salt = Convert.ToBase64String(saltBytes);
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, iterations, HashAlgorithmName.SHA256))
        {
            byte[] hashBytes = pbkdf2.GetBytes(hashSize);
            string hash = Convert.ToBase64String(hashBytes);
            return $"pbkdf2_sha256${iterations}${salt}${hash}";
        }
    }


    public bool VerifyPassword(string password)
    {
        string storedPassword = this.Password;
        if (string.IsNullOrEmpty(storedPassword))
            return false;
        var parts = storedPassword.Split('$');
        if (parts.Length != 4)
            return false;

        string algorithm = parts[0];
        if (!algorithm.Equals("pbkdf2_sha256", StringComparison.OrdinalIgnoreCase))
            return false;

        if (!int.TryParse(parts[1], out int iterations))
            return false;

        string salt = parts[2];
        string expectedHash = parts[3];
        byte[] saltBytes = Convert.FromBase64String(salt);
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, iterations, HashAlgorithmName.SHA256))
        {
            byte[] hashBytes = pbkdf2.GetBytes(32);
            string computedHash = Convert.ToBase64String(hashBytes);
            return computedHash == expectedHash;
        }
    }

    public void SetPassword(string password)
    {
        this.Password = PasswordHash(password);
    }
}

