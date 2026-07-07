using System.Security.Cryptography;
using System.Text;

namespace TalentShowcase.Api.Helpers
{
    public static class TokenHelper
    {
        // A long, random, URL-safe token that goes into the email link.
        public static string GenerateRawToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        // We only store this hash in the DB, never the raw token. On verify we
        // re-hash the incoming token and look it up. SHA256 is enough here
        // because the token is already long and random (unlike a password).
        public static string Hash(string rawToken)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }
    }
}
