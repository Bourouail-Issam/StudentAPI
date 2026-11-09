using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StudentAPIBusinessLayer
{
    public class RefreshTokenService : IRefreshTokenService
    {
        public string GenerateRefreshToken()
            => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        /// <summary>
        /// SHA-256 without salt: the token is already 512 bits of randomness,
        /// and a deterministic hash lets us look it up by index.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Hash refresh token</returns>
        // 
        public string HashRefreshToken(string token)
        {
            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(hash); // always 44 characters
        }

    }
}
