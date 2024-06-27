using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;
using SecretsSharing.Infrastructure;

namespace SecretsSharing.WebApi.Controllers;

[Route("api/files")]
public class FilesController : ControllerBase
{
    private PostgresDbContext postgresDbContext;
    private readonly S3StorageHandler s3StorageHandler;

    public FilesController(PostgresDbContext postgresDbContext, S3StorageHandler s3StorageHandler)
    {
        this.postgresDbContext = postgresDbContext;
        this.s3StorageHandler = s3StorageHandler;
    }

    [HttpPost("Upload")]
    public async Task<ActionResult<string>> UploadFile(IFormFile formFile)
    {
        // var putObjectResponse = await minioClient.PutObjectAsync(new PutObjectArgs()
        //     .WithBucket("secrets-sharing-drive")
        //     .WithStreamData(formFile.OpenReadStream())
        //     .WithObject(formFile.FileName)
        //     .WithObjectSize(formFile.OpenReadStream().Length));

        await s3StorageHandler.Upload(formFile.OpenReadStream(), formFile.FileName);

        return Ok();
    }

    [HttpGet("users/{login}/password")]
    public ActionResult<string> GetUserPassword(string login)
    {
        return Ok(postgresDbContext.Users.FirstOrDefault(user => user.Login == login));
    }
}