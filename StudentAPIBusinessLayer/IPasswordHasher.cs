using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentAPIBusinessLayer
{
    public interface IPasswordHasher 
    {
        string HashPassword(string plainText);
        bool VerifyPassword(string password, string storedHash);
    }
}
