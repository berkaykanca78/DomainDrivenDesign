using DomainDrivenDesign.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DomainDrivenDesign.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly LoginSettings _loginSettings;

        public AuthController(IOptions<JwtSettings> jwtOptions, IOptions<LoginSettings> loginOptions)
        {
            _jwtSettings = jwtOptions.Value;
            _loginSettings = loginOptions.Value;
        }

        [HttpPost(nameof(Login))]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request.Username != _loginSettings.Username || request.Password != _loginSettings.Password)
                return Unauthorized("Kullanıcı adı veya şifre hatalı");

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, request.Username),
                new Claim(ClaimTypes.Role, "Admin"),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }
}
