using DomainDrivenDesign.Infrastructure.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace DomainDrivenDesign.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CryptoController : ControllerBase
{
    [HttpPost("encrypt")]
    public IActionResult Encrypt([FromBody] string text)
    {
        if (string.IsNullOrEmpty(text))
            return BadRequest("Metin boş olamaz.");

        try
        {
            var encryptedText = CryptoHelper.Encrypt(text);
            return Ok(encryptedText);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Şifreleme işlemi sırasında bir hata oluştu: {ex.Message}");
        }
    }

    [HttpPost("decrypt")]
    public IActionResult Decrypt([FromBody] string text)
    {
        if (string.IsNullOrEmpty(text))
            return BadRequest("Metin boş olamaz.");

        try
        {
            var decryptedText = CryptoHelper.Decrypt(text);
            return Ok(decryptedText);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Şifre çözme işlemi sırasında bir hata oluştu: {ex.Message}");
        }
    }
} 