using System.Security.Cryptography;

namespace ProductManagment_APIs.Service
{
    public static class PasswordHelper
    {
        public static byte[] HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        public static bool VerifyPassword(string password, byte[] hash)
        {
            var computedHash = HashPassword(password);
            return computedHash.SequenceEqual(hash);
        }
    }
}
