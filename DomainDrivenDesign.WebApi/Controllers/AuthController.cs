using DomainDrivenDesign.WebApi.Helpers;
using DomainDrivenDesign.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DomainDrivenDesign.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly LoginSettings _loginSettings;
        private readonly string _decryptedUsername;
        private readonly string _decryptedPassword;

        public AuthController(IOptions<JwtSettings> jwtOptions, IOptions<LoginSettings> loginOptions)
        {
            _jwtSettings = jwtOptions.Value;
            _loginSettings = loginOptions.Value;

            try
            {
                _decryptedUsername = EncryptionHelper.Decrypt(_loginSettings.Username);
                _decryptedPassword = EncryptionHelper.Decrypt(_loginSettings.Password);
            }
            catch (CryptographicException ex)
            {
                _decryptedUsername = string.Empty;
                _decryptedPassword = string.Empty;
            }
            catch (Exception ex)
            {
                _decryptedUsername = string.Empty;
                _decryptedPassword = string.Empty;
            }
        }

        [HttpPost(nameof(Login))]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(_decryptedUsername) || string.IsNullOrEmpty(_decryptedPassword))
            {
                return StatusCode(500, "Sistem yapılandırma hatası. Lütfen yönetici ile iletişime geçin.");
            }

            if (request.Username != _decryptedUsername || request.Password != _decryptedPassword)
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
