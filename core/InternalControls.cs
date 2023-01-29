using System.Text.RegularExpressions;
using core.Data;

namespace core;

public class InternalControls
{
    private readonly string[] _randomChars = {
        "ABCDEFGHJKLMNOPQRSTUVWXYZ",
        "abcdefghijkmnopqrstuvwxyz",
        "0123456789",
        "!@$?_-"
    };

    private readonly IServiceProvider _serviceProvider;
    
    public InternalControls(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
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
        Console.WriteLine("Root user credentials(Username | Password): root | "+unencrypted);
        
        new Thread(ConsoleReader).Start();
    }

    private string GeneratePass(out string unencrypted)
    {
        var rand = new Random(Environment.TickCount);
        var chars = new List<char>();
        for (var i = 0; i < 10; i++)
        {
            var row = rand.Next(0, 3);
            chars.Add(_randomChars[row][rand.Next(0, _randomChars[row].Length)]);
        }

        unencrypted = new string(chars.ToArray());
        return CryptPass(unencrypted);
    }

    private string CryptPass(string pass) => BCrypt.Net.BCrypt.HashPassword(pass);

    private string RandomizeOrNot(string password, out string? actualPass)
    {
        var isRand = password.Equals("$random");
        
        if (isRand) return GeneratePass(out actualPass);

        actualPass = password;
        return CryptPass(password);
    }

    private void ConsoleReader()
    {
        Thread.Sleep(1000);
        Console.WriteLine("You can register new user by typing '/reg `login` `password` `role(admin|user)`'.\nType `$random` in password field to make it random.");
        while (true)
        {
            var t = Console.ReadLine();
            var fields = Regex.Match(t, @"\/reg\s(.+)\s(.+)\s(.+)");
            var username = fields.Groups[1].Value;
            var password = fields.Groups[2].Value;
            var role = fields.Groups[3].Value;

            if (!Enum.TryParse(role, true, out Roles actualRole))
            {
                Console.WriteLine("Unable to find role, try again.");
            }
            else
            {
                var user = new UserEntity(username, RandomizeOrNot(password, out var actualPass), actualRole);

                var db = _serviceProvider.GetRequiredService<DatabaseContext>();
                try
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                
                    Thread.Sleep(100);
                    Console.WriteLine("User registered: " + username + " | " + actualPass);
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine("User with this username already exist in database.");
                }
            }
        }
    }
}