using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Minio;
using SecretsSharing.Application;
using SecretsSharing.Application.Services;
using SecretsSharing.Infrastructure;
using SecretsSharing.Infrastructure.Models;

//using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCors();

AddUserIdentity(builder);

builder.Services.AddDbContext<PostgresDbContext>(optionsBuilder =>
{
    var dbConnectionString = builder.Configuration.GetConnectionString("WebApi");
    optionsBuilder.UseNpgsql(dbConnectionString);
});

builder.Services.AddScoped<UserService>();

AddS3StorageHandler(builder);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();

void AddUserIdentity(WebApplicationBuilder webApplicationBuilder)
{
    webApplicationBuilder.Services.AddScoped<UserIdentityService>(sp =>
    {
        var httpContextAccessor = sp.GetService<IHttpContextAccessor>();

        if (httpContextAccessor == null ||
            httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated is false)
        {
            return new UserIdentityService();
        }

        var claimsPrincipal = httpContextAccessor.HttpContext!.Request.HttpContext.User;
        var identity = new UserIdentityService(claimsPrincipal);

        return identity;
    });
}

void AddS3StorageHandler(WebApplicationBuilder builder1)
{
    builder1.Services.AddSingleton<S3StorageHandler>(_ =>
    {
        var amazonS3Config = new AmazonS3Config
        {
            ServiceURL = "http://localhost:9000"
        };

        var s3Client = new AmazonS3Client("8hjgIVKJHmUGDYWzw56c",
            "dFSTj1MwG4jpI29nzWOuGd6xSvyjLzRbAApypFDl",
            amazonS3Config);

        var transferUtility = new TransferUtility(s3Client);

        return new S3StorageHandler(transferUtility);
    });
}