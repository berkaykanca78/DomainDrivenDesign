using DomainDrivenDesign.Application.Models;
using DomainDrivenDesign.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DomainDrivenDesign.WebApi.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class FileController(FileManager fileManager) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> UploadImage([FromForm] UploadImageDto request)
    {
        var response = await fileManager.UploadImageAsync(Guid.NewGuid(), request.File);
        return Ok(response.HttpStatusCode);
    }

    [HttpGet]
    public async Task<IActionResult> Get(Guid id)
    {
        try
        {
            var response = await fileManager.GetImageAsync(id);
            return File(response.ResponseStream, response.Headers.ContentType);
        }
        catch (Exception ex) when (ex.Message is "The specified key does not exists")
        {
            return NotFound();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await fileManager.DeleteImageAsync(id);
        return response.HttpStatusCode switch
        {
            HttpStatusCode.NoContent => Ok(),
            HttpStatusCode.NotFound => NotFound(),
            _ => BadRequest()
        };
    }
}
