using System.ComponentModel.DataAnnotations;

namespace StudentAPI.Auth
{
    public class LogoutRequest
    {
        public required string RefreshToken { get; set; }
        public required string Email { get; set; }
    }
}
