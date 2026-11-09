using SharedDTOModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAPIBusinessLayer
{
    public interface IUser
    {
        Task<UserDTO?> GetUserByEmailAsync(string Email);
        Task<UserDTO?> GetUserByIdAsync(int userId);
        Task SaveRefreshTokenAsync(int userId, string refreshTokenHash, DateTime expiresAtUtc);
        Task<UserDTO?> RotateRefreshTokenAsync(string oldHash, string newHash, DateTime newExpiresAtUtc);
        Task RevokeRefreshTokenAsync(int userId);
    }
}
