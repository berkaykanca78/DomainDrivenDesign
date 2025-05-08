using Microsoft.AspNetCore.Http;

namespace DomainDrivenDesign.Application.Models;

public sealed record UploadImageDto(
    IFormFile File); 