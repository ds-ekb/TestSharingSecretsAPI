using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecretsSharing.Infrastructure.Models;

[Table("users")]
public class User
{
    [Column("id"), Key, Required]
    public long Id { get; set; }
    [Column("login"), Required]
    public string Login { get; set; }
    [Column("password"), Required]
    public string Password { get; set; }
}