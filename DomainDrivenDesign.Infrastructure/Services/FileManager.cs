using Amazon.S3;
using Amazon.S3.Model;
using DomainDrivenDesign.Infrastructure.Settings;
using Microsoft.AspNetCore.Http;

namespace DomainDrivenDesign.Infrastructure.Services;

public class FileManager(IAmazonS3 s3, S3Settings s3Settings)
{
    private readonly IAmazonS3 _s3 = s3;
    private readonly S3Settings _s3Settings = s3Settings;

    public async Task<PutObjectResponse> UploadImageAsync(Guid id, IFormFile file)
    {
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = _s3Settings.BucketName,
            Key = $"{_s3Settings.DirectoryName}/{id}",
            ContentType = file.ContentType,
            InputStream = file.OpenReadStream(),
            Metadata =
            {
                ["x-amz-meta-originalname"] = file.FileName,
                ["x-amz-meta-extension"] = Path.GetExtension(file.FileName)
            }
        };

        return await _s3.PutObjectAsync(putObjectRequest);
    }

    public async Task<GetObjectResponse> GetImageAsync(Guid id)
    {
        var getObjectRequest = new GetObjectRequest
        {
            BucketName = _s3Settings.BucketName,
            Key = $"{_s3Settings.DirectoryName}/{id}",
        };

        return await _s3.GetObjectAsync(getObjectRequest);
    }

    public async Task<DeleteObjectResponse> DeleteImageAsync(Guid id)
    {
        var deleteObjectRequest = new DeleteObjectRequest
        {
            BucketName = _s3Settings.BucketName,
            Key = $"{_s3Settings.DirectoryName}/{id}",
        };

        return await _s3.DeleteObjectAsync(deleteObjectRequest);
    }
} 