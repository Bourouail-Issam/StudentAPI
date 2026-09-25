using System;
using System.Security.Cryptography;


namespace SharedDTOModel
{
    public class PasswordHasher
    {

        // ── Cryptography Constants ────────────────────────────────────────────
        /// <summary>Size of the random salt in bytes (NIST recommendation: minimum 16)</summary>
        private const int SaltSize = 16;

        /// <summary>Size of the derived hash in bytes (SHA-256 = 32 bytes)</summary>
        private const int HashSize = 32;

        /// <summary>
        /// PBKDF2 iteration count.
        /// OWASP 2023 recommendation: minimum 600,000 for SHA-256.
        /// Using 100,000 as a balanced default — increase for higher security environments.
        /// </summary>
        private const int Iteration = 600000;

  
        #region  Password Hashing (PBKDF2 / SHA-256)

        /// <summary>
        /// Generates a cryptographically secure random salt using RNGCryptoServiceProvider.
        /// </summary>
        /// <returns>A byte array of length <see cref="SaltSize"/>, or null if generation fails</returns>
        private static byte[]? GenerateSalt()
        {
            byte[] salt = new byte[SaltSize];
            try
            {
                // RandomNumberGenerator is the recommended CSPRNG in .NET
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                    return salt;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Hashes a plain-text password using PBKDF2 with SHA-256 and a random salt.
        /// Output format: Base64( Salt[16] + Hash[32] ) → always 64 characters.
        /// </summary>
        /// <param name="PlainText">The plain-text password to hash</param>
        /// <returns>
        /// A 64-character Base64 string containing salt + hash,
        /// or null if input is empty or an internal error occurs
        /// </returns>
        public static string? HashPassword(string PlainText)
        {
            if (string.IsNullOrWhiteSpace(plainText))
                throw new ArgumentException("Password cannot be empty.", nameof(plainText));

            // Generate a fresh random salt for every password
            byte[]? salt = GenerateSalt();

            if (salt == null)
                return null;

            using (var PDKFD2 = new Rfc2898DeriveBytes(PlainText, salt, Iteration, HashAlgorithmName.SHA256))
            {
                byte[] hash = PDKFD2.GetBytes(HashSize);

                // Combine salt + hash into one array for compact storage
                byte[] combined = new byte[HashSize + SaltSize];  // 16 + 32 = 48 bytes
                Array.Copy(salt, 0, combined, 0, SaltSize); // bytes  0–15  = salt
                Array.Copy(hash, 0, combined, SaltSize, HashSize); // bytes 16–47  = hash

                // Encode to Base64 → 64 characters
                return Convert.ToBase64String(combined);
            }
        }

        /// <summary>
        /// Verifies a plain-text password against a stored PBKDF2 hash.
        /// Extracts the original salt from the stored hash, recomputes the hash,
        /// then uses constant-time comparison to prevent timing attacks.
        /// </summary>
        /// <param name="password">The plain-text password entered by the user</param>
        /// <param name="storedHash">The Base64-encoded hash previously created by <see cref="HashPassword"/></param>
        /// <returns>True if the password matches; false otherwise</returns>
        public static bool VerifyPassword(string password, string storedHash)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            if (string.IsNullOrEmpty(storedHash))
                return false;

            try
            {
                // Decode Base64 → 48 bytes (16 salt + 32 hash)
                byte[] combined = Convert.FromBase64String(storedHash);

                // ✅ Validate expected length before array operations
                if (combined.Length != SaltSize + HashSize)
                {
                    throw new ArgumentException($"VerifyPassword: Invalid storedHash length ({combined.Length} bytes)."); ;
                }
                // Extract the original salt (first 16 bytes)
                byte[] salt = new byte[SaltSize];
                Array.Copy(combined, 0, salt, 0, SaltSize);

                // Extract the stored hash (next 32 bytes)
                byte[] storedPasswordHash = new byte[HashSize];
                Array.Copy(combined, SaltSize, storedPasswordHash, 0, HashSize);

                // Recompute hash using the same salt and iteration count
                using (var PDKFD2 = new Rfc2898DeriveBytes(password, salt, Iteration, HashAlgorithmName.SHA256))
                {
                    byte[] computeHash = PDKFD2.GetBytes(HashSize);

                    // ✅ Constant-time comparison to prevent timing attacks
                    return CryptographicOperations.FixedTimeEquals(computeHash, storedPasswordHash);
                }
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("VerifyPassword: storedHash is not valid Base64.");
            }
            catch (CryptographicException ex)
            {
                throw new ArgumentException("VerifyPassword: Cryptographic failure.");
            }
            catch (Exception ex)
            {
                throw new ArgumentException("VerifyPassword: Unexpected error.");
            }
        }

        #endregion
    }
}
