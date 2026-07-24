using g2soir.Models;
using g2soir.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace g2soir.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IServices _service;
        private readonly IConfiguration _config;

        public AuthController(IServices service, IConfiguration config)
        {
            _service = service;
            _config = config;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            var existing = _service.GetUserByEmail(user.Email);
            if (existing != null)
                return BadRequest(new { message = "Email déjà utilisé" });

            _service.Add(user);
            return Ok(new { message = "Compte créé avec succès" });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            var user = _service.authentificat(dto.Email, dto.Password);
            if (user == null)
                return Unauthorized(new { message = "Email ou mot de passe incorrect" });

            var token = GenerateToken(user);
            return Ok(new
            {
                token,
                user = new { user.Id, user.Nom, user.Prenom, user.Email, user.Role }
            });
        }

        private string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "g2soir_secret_key_32chars_minimum!"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Nom),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: "g2soir",
                audience: "g2soir",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginDto
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
