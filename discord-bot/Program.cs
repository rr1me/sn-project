using discordBot;
var builder = WebApplication.CreateBuilder();

await new BotRunner().Run(builder);