using SharedDTOModel;
using StudentDataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAPIBusinessLayer
{
    public class User : IUser
    {
        private readonly UserData _userData;
        public User(string connectionString)
        {
            _userData = new UserData(connectionString);
        }
        public Task<UserDTO?> GetUserByEmailAsync(string email)
              => _userData.GetUserByEmailAsync(email);

        public Task<UserDTO?> GetUserByIdAsync(int userId)
            => _userData.GetUserByIdAsync(userId);

        public Task SaveRefreshTokenAsync(int userId, string refreshTokenHash, DateTime expiresAtUtc)
            => _userData.SaveRefreshTokenAsync(userId, refreshTokenHash, expiresAtUtc);

        public Task<UserDTO?> RotateRefreshTokenAsync(string oldHash, string newHash, DateTime newExpiresAtUtc)
            => _userData.RotateRefreshTokenAsync(oldHash, newHash, newExpiresAtUtc);

        public Task RevokeRefreshTokenAsync(int userId)
            => _userData.RevokeRefreshTokenAsync(userId);
    }
}
