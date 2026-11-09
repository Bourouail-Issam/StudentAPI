namespace StudentAPI.Auth
{
    //public record AuthResponse(string AccessToken, string RefreshToken);
    public class RefreshRequest
    {
        public required string RefreshToken { get; set; } = string.Empty;
    }
}
