using Microsoft.EntityFrameworkCore;
using SecretsSharing.Infrastructure.Models;

namespace SecretsSharing.Infrastructure;

public class PostgresDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public PostgresDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
    {

    }
}