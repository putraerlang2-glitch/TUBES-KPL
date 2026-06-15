using System;
using System.Security.Cryptography;
using System.Text;

namespace ObatAPI.Helpers
{
    /// <summary>
    /// Helper for hashing and verifying passwords (SHA256 + salt, konsisten dengan WinForms)
    /// </summary>
    public static class PasswordHasher
    {
        private const int SaltSize = 16;  // 128-bit salt
        private const int HashSize = 20;  // 160-bit hash (compatible with Rfc2898DeriveBytes)
        private const int Iterations = 10000;

        /// <summary>
        /// Hash a password with a random salt (return combined salt + hash as base64 string)
        /// </summary>
        public static string HashPassword(string password)
        {
            using var deriveBytes = new Rfc2898DeriveBytes(password, SaltSize, Iterations, HashAlgorithmName.SHA256);
            byte[] salt = deriveBytes.Salt;
            byte[] hash = deriveBytes.GetBytes(HashSize);

            // Combine salt and hash into one byte array
            byte[] hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Verify a password against a stored hash (from HashPassword)
        /// </summary>
        public static bool VerifyPassword(string password, string storedHashBase64)
        {
            byte[] hashBytes = Convert.FromBase64String(storedHashBase64);

            // Extract salt and hash from the stored bytes
            byte[] salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);

            using var deriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] computedHash = deriveBytes.GetBytes(HashSize);

            // Compare computed hash with stored hash
            for (int i = 0; i < HashSize; i++)
            {
                if (hashBytes[SaltSize + i] != computedHash[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
