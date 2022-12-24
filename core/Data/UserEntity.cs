using System.ComponentModel.DataAnnotations;

namespace core.Data;

public class UserEntity
{
    // [Key]
    // public int Id { get; set; }
    [Key]
    public string Username { get; set; }
    public string Password { get; set; }
    public Roles Role { get; set; }
    public string? RefreshToken { get; set; }

    public UserEntity(string username, string password, Roles role)
    {
        Username = username;
        Password = password;
        Role = role;
    }
}