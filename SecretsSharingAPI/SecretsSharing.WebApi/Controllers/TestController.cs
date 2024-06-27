using Microsoft.AspNetCore.Mvc;
using SecretsSharing.Infrastructure;

namespace SecretsSharing.WebApi.Controllers;

[Route("api")]
public class TestController : ControllerBase
{
    private PostgresDbContext postgresDbContext;

    public TestController(PostgresDbContext postgresDbContext)
    {
        this.postgresDbContext = postgresDbContext;
    }

    [HttpPost("Upload")]
    public ActionResult<string> UploadFile(IFormFile formFile)
    {
        return Ok(formFile.FileName);
    }

    [HttpGet("users/{login}/password")]
    public ActionResult<string> GetUserPassword(string login)
    {
        return Ok(postgresDbContext.Users.FirstOrDefault(user => user.Login == login));
    }
}