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

        public UserDTO() { }
        public UserDTO(int UserId, string email, string passwordHash, string role)
        {
            this.UserId = UserId;
            this.Email = email;
            this.PasswordHash = passwordHash;
            this.Role = role;
        }
         
    }
}
