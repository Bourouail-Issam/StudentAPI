namespace StudentAPI.Auth
{
    public interface  IJwtTokenService
    {
        string CreateAccessToken(int userId, string email, string role);
    }
}
