/*using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChatAppBackend.Models;
using ChatAppBackend.Data;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authorization;

namespace ChatAppBackend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly MongoDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(MongoDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _context.Employees
                .Find(emp => emp.Username == request.Username && emp.Password == request.Password)
                .FirstOrDefault();

            if (user == null)
                return Unauthorized(new { message = "Nom d'utilisateur ou mot de passe incorrect." });

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role ?? "User"),
        new Claim("employeIdSql", user.EmployeIdSql ?? "")
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                userId = user.Id,
                username = user.Username,
                role = user.Role ?? "User",
                employeId = user.EmployeIdSql
            });
        }

        [Authorize]
        [HttpGet("currentUser")]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var employeId = User.FindFirst("employeIdSql")?.Value;

            return Ok(new { userId, username, role, employeId });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
*/