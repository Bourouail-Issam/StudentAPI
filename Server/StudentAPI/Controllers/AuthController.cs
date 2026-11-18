using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using StudentAPI.Auth;
using StudentAPIBusinessLayer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace StudentApi.Controllers
{
    // This controller is responsible for authentication-related actions,
    // such as logging in and issuing JWT tokens.
    [Route("Api/Auth")]
    [ApiController]
    [Produces("application/json")]

    public class AuthController : ControllerBase
    {
        // Used to equalize response time when the email does not exist
        private static readonly string DummyPasswordHash =
            new PasswordHasher().HashPassword("dummy-password-for-timing");

        private readonly IUser _user;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IConfiguration _configuration;

        public AuthController(
            IUser user, 
            IPasswordHasher passwordHasher,
            IRefreshTokenService refreshTokenService,
            IJwtTokenService jwtTokenService,
            IConfiguration configuration)
        {
            _user= user;
            _passwordHasher = passwordHasher;
            _refreshTokenService = refreshTokenService;
            _jwtTokenService = jwtTokenService;
            _configuration = configuration;
        }

        private int RefreshTokenDays =>
         _configuration.GetValue<int?>("Jwt:RefreshTokenDays") ?? 7;

        // This endpoint handles user login.
        // It verifies credentials and returns a JWT token if login succeeds.
        [HttpPost("login")]
        [EnableRateLimiting("AuthLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Step 1: Find the student by email from the in- DataBase
            // Email acts as the unique login identifier.
            var user = await _user.GetUserByEmailAsync(request.Email);


            // If no student is found with the given email,
            // return 401 Unauthorized without revealing which field was wrong.
            if (user == null)
            {
                _passwordHasher.VerifyPassword(request.Password, DummyPasswordHash);
                return Unauthorized("Invalid credentials");

            }

            if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials");

            var accessToken = _jwtTokenService.CreateAccessToken(user.UserId,user.Email,user.Role);
            var refreshToken = _refreshTokenService.GenerateRefreshToken();

            await _user.SaveRefreshTokenAsync(
                user.UserId,
                _refreshTokenService.HashRefreshToken(refreshToken),
                DateTime.UtcNow.AddDays(RefreshTokenDays));

            // The RAW refresh token goes to the client; only its hash is stored
            return Ok(new TokenResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                }
            );
        }

        [HttpPost("refresh")]
        [EnableRateLimiting("AuthLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return BadRequest("Refresh token is required.");

            var oldHash = _refreshTokenService.HashRefreshToken(request.RefreshToken);

            var newRefreshToken = _refreshTokenService.GenerateRefreshToken();

            var user = await _user.RotateRefreshTokenAsync(
                 oldHash, newRefreshToken, DateTime.UtcNow.AddDays(RefreshTokenDays));

            if (user == null)
                return Unauthorized("Invalid refresh token");

            var accessToken = _jwtTokenService.CreateAccessToken(user.UserId, user.Email, user.Role);

            return Ok(new TokenResponse
            { 
                AccessToken = accessToken,
                RefreshToken = newRefreshToken
            });
        }


        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout()
        {
            var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (!int.TryParse(sub, out int userId))
                return Unauthorized();

            await _user.RevokeRefreshTokenAsync(userId);
            return NoContent();
        }
    }
}