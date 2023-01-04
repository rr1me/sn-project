using discordBot;

var builder = WebApplication.CreateBuilder();

builder.Services.AddControllers();
builder.Services.AddHttpClient();

await new BotRunner(builder).Run();