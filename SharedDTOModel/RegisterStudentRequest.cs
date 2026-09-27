using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDTOModel
{
    public class RegisterStudentRequest
    {
        public StudentDTO Student { get; set; } = new StudentDTO();
        public UserDTO User{ get; set; } = new UserDTO();
    }
}
