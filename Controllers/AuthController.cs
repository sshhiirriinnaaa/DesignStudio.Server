using DesignStudio.Server.Data;
using DesignStudio.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DesignStudio.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
       
            var admin = _context.AdminUsers.FirstOrDefault(a => a.Username == request.Username);

          
            if (admin == null || !BCrypt.Net.BCrypt.Verify(request.Password, admin.PasswordHash))
            {
                return Unauthorized(new { message = "Неверный логин или пароль" });
            }
            var keyStr = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

          
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, admin.Username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var token = new JwtSecurityToken(
                issuer: "StoryHome",
                audience: "StoryHomeAdmin",
                claims: claims,
                expires: DateTime.Now.AddHours(12),
                signingCredentials: credentials);

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }


        [HttpPost("register-first-admin")]
        public IActionResult RegisterAdmin([FromBody] LoginRequest request)
        {
            if (_context.AdminUsers.Any(a => a.Username == request.Username))
            {
                return BadRequest("Пользователь с таким логином уже существует.");
            }

            // Хэшируем пароль перед сохранением в БД
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newAdmin = new AdminUser
            {
                Username = request.Username,
                PasswordHash = passwordHash
            };

            _context.AdminUsers.Add(newAdmin);
            _context.SaveChanges();

            return Ok(new { message = "Администратор успешно создан!" });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}