using discordBot;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(x =>
{
    x.AddFile("app.log", x=>x.MinLevel = LogLevel.Debug);
});

builder.Services.AddControllers();
builder.Services.AddHttpClient();

await new BotRunner(builder).Run();