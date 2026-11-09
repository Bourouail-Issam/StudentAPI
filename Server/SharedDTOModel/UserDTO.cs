using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDTOModel
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public  string Email { get; set; } = string.Empty;
        public  string PasswordHash { get; set; } = string.Empty;
        public  string Role { get; set; } = string.Empty;

        // Refresh token fields
        public string? RefreshTokenHash { get; set; }
        public DateTime? RefreshTokenExpiresAt { get; set; }
        public DateTime? RefreshTokenRevokedAt { get; set; }

        public UserDTO() { }
        public UserDTO(int userId, string email, string passwordHash, string role,
                 string? refreshTokenHash = null,
                 DateTime? refreshTokenExpiresAt = null, 
                 DateTime? refreshTokenRevokedAt = null)
        {
            UserId = userId;
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
            RefreshTokenHash = refreshTokenHash ?? string.Empty;
            RefreshTokenExpiresAt = refreshTokenExpiresAt;
            RefreshTokenRevokedAt = refreshTokenRevokedAt;
        }
         
    }
}
