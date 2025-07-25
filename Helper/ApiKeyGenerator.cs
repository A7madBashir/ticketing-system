using System.Security.Cryptography;

namespace TicketingSystem.Helper;

public static class ApiKeyGenerator
{
    public static string Generate(int length = 20)
    {
        var bytes = new byte[length];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }

        // Convert to a URL-safe Base64 string (replaces '+' with '-' and '/' with '_')
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_");
    }
}
