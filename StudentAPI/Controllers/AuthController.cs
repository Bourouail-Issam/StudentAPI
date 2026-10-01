using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StudentAPI.Model;
using StudentAPIBusinessLayer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace StudentApi.Controllers
{
    // This controller is responsible for authentication-related actions,
    // such as logging in and issuing JWT tokens.
    [Route("StudentApi/Auth")]
    [ApiController]
    [Produces("application/json")]

    public class AuthController : ControllerBase
    {
        private readonly IUser _User;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IConfiguration _configuration;
        public AuthController(IUser user, IPasswordHasher passwordHasher, IConfiguration configuration)
        {
            _User= user;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        // This endpoint handles user login.
        // It verifies credentials and returns a JWT token if login succeeds.
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Step 1: Find the student by email from the in- DataBase
            // Email acts as the unique login identifier.
            var user = await _User.GetUserByEmailAsync(request.Email);


            // If no student is found with the given email,
            // return 401 Unauthorized without revealing which field was wrong.
            if (user == null)
                return Unauthorized("Invalid credentials");


            // Step 2: Verify the provided password against the stored hash.

            bool isValidPassword = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);


            // If the password does not match the stored hash,
            // return 401 Unauthorized.
            if (!isValidPassword)
                return Unauthorized("Invalid credentials");


            // Step 3: Create claims that represent the authenticated user's identity.
            // These claims will be embedded inside the JWT.
            var claims = new[]
            {
                // Unique identifier for the student
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),


                // Student email address
                new Claim(JwtRegisteredClaimNames.Email, user.Email),


                // Role (Student or Admin) used later for authorization
                new Claim(ClaimTypes.Role, user.Role)
            };


            // Step 4: the symmetric security key used to sign the JWT.
            // This key must match the key used in JWT validation middleware.
            var jwtKey = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("Jwt:Key is not configured.");


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)) ;


            // Step 5: Define the signing credentials.
            // This specifies the algorithm used to sign the token.
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            // Step 6: Create the JWT token.
            // The token includes issuer, audience, claims, expiration, and signature.
            var issuer = _configuration["Jwt:Issuer"] 
                ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");

            var audience = _configuration["Jwt:Audience"] 
                ?? throw new InvalidOperationException("Jwt:Audience is not configured.");

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );


            // Step 7: Return the serialized JWT token to the client.
            // The client will send this token with future requests.
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }
}