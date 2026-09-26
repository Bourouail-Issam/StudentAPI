using System;
using System.Security.Cryptography;

namespace StudentAPIBusinessLayer
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iteration = 600000;  // OWASP recommendation

        public static string HashPassword(string plainText)
        {
            if (string.IsNullOrWhiteSpace(plainText))
                throw new ArgumentException("Password cannot be empty.", nameof(plainText));

            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            using (var pbkdf2 = new Rfc2898DeriveBytes(plainText, salt, Iteration, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(HashSize);

                // Format: [4 bytes Iteration][16 bytes Salt][32 bytes Hash]
                byte[] combined = new byte[4 + SaltSize + HashSize];
                BitConverter.GetBytes(Iteration).CopyTo(combined, 0);
                Array.Copy(salt, 0, combined, 4, SaltSize);
                Array.Copy(hash, 0, combined, 4 + SaltSize, HashSize);

                return Convert.ToBase64String(combined);
            }
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash))
                return false;

            byte[] combined = Convert.FromBase64String(storedHash);

            if (combined.Length != 4 + SaltSize + HashSize)
                throw new ArgumentException("Invalid stored hash format.");

            int iterations = BitConverter.ToInt32(combined, 0);

            byte[] salt = new byte[SaltSize];
            Array.Copy(combined, 4, salt, 0, SaltSize);

            byte[] storedPasswordHash = new byte[HashSize];
            Array.Copy(combined, 4 + SaltSize, storedPasswordHash, 0, HashSize);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                byte[] computedHash = pbkdf2.GetBytes(HashSize);
                return CryptographicOperations.FixedTimeEquals(computedHash, storedPasswordHash);
            }
        }
    }
}