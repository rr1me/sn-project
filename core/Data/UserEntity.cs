using System.ComponentModel.DataAnnotations;

namespace core.Data;

public class UserEntity
{
    [Key]
    public int Id { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public Roles Role { get; set; }
    public string? RefreshToken { get; set; }

    public UserEntity(string login, string password, Roles role)
    {
        Login = login;
        Password = password;
        Role = role;
    }
}