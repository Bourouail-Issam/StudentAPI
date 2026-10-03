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
        public async Task<UserDTO?> GetUserByEmailAsync(string Email)
        {
            return await _userData.GetUserByEmailAsync(Email);
        }
    }
}
