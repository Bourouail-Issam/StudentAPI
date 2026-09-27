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
        public int? StudentId { get; set; }

        public UserDTO() { }
        public UserDTO(int id, string email, string passwordHash, string role, int studentId)
        {
            this.UserId = id;
            this.Email = email;
            this.PasswordHash = passwordHash;
            this.Role = role;
            this.StudentId = studentId;
        }
         
    }
}
