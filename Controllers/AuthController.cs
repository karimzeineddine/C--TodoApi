using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly string _secretKey;

public AuthController(IConfiguration config)
{
    _secretKey = config["Jwt:SecretKey"] ?? throw new ArgumentNullException("Jwt:SecretKey", "Secret key is not configured in appsettings.json.");
}


    [HttpPost("login")]
    public IActionResult Login([FromBody] UserLogin login)
    {
        if (login.Username == "admin" && login.Password == "password")
        {
            var key = Encoding.UTF8.GetBytes(_secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim("sub", login.Username),
                    new System.Security.Claims.Claim("role", "Admin")
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = "TodoApi",
                Audience = "TodoApiClients",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new { Token = tokenHandler.WriteToken(token) });
        }

        return Unauthorized();
    }
}
