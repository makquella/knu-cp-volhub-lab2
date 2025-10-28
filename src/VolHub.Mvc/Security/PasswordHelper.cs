using System.Security.Cryptography;
using System.Text;

namespace VolHub.Mvc.Security;

public static class PasswordHelper
{
    public static string HashPassword(string password)
    {
        if (password is null)
        {
            throw new ArgumentNullException(nameof(password));
        }

        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
