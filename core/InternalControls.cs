using core.Data;

namespace core;

public class InternalControls
{
    private readonly string[] _randomChars =
    {
        "ABCDEFGHJKLMNOPQRSTUVWXYZ",
        "abcdefghijkmnopqrstuvwxyz",
        "0123456789",
        "!@$?_-"
    };

    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InternalControls> _logger;

    public InternalControls(IServiceProvider serviceProvider, ILogger<InternalControls> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public void Initialize()
    {
        var db = _serviceProvider.GetRequiredService<DatabaseContext>();

        db.Database.EnsureCreated();

        var rootUser = db.Users.FirstOrDefault(x => x.Username == "root");

        var password = GeneratePass(out var unencrypted);

        if (rootUser == null)
            db.Users.Add(new UserEntity("root", password, Roles.Admin));
        else
        {
            rootUser.RefreshToken = null;
            rootUser.Password = password;
            db.Users.Update(rootUser);
        }

        db.SaveChanges();
        _logger.LogInformation("Root user credentials(Username | Password): root | " + unencrypted);
    }

    private string GeneratePass(out string unencrypted)
    {
        var rand = new Random(Environment.TickCount);
        var chars = new List<char>();
        for (var i = 0; i < 10; i++)
        {
            var row = rand.Next(0, 4);
            chars.Add(_randomChars[row][rand.Next(0, _randomChars[row].Length)]);
        }

        unencrypted = new string(chars.ToArray());
        return CryptPass(unencrypted);
    }

    private string CryptPass(string pass) => BCrypt.Net.BCrypt.HashPassword(pass);
}