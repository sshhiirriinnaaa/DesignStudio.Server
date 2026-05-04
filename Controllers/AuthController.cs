using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace DesignStudio.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Хардкод для диплома. В идеале - проверка через базу данных.
            if (request.Username == "admin" && request.Password == "admin123")
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKeyForStoryHomeDiplomaProject2026!"));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "StoryHome",
                    audience: "StoryHomeAdmin",
                    expires: DateTime.Now.AddHours(2), // Токен живет 2 часа
                    signingCredentials: credentials);

                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }

            return Unauthorized("Неверный логин или пароль");
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}