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
    }
}
