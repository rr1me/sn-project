using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using discordBot.Data;

namespace discordBot;

public class BotRunner
{
    private readonly WebApplicationBuilder _builder;
    
    public BotRunner(WebApplicationBuilder builder)
    {
        var config = new DiscordSocketConfig()
        {
            GatewayIntents =
                GatewayIntents.GuildMessageReactions |
                GatewayIntents.GuildMembers |
                GatewayIntents.GuildPresences |
                GatewayIntents.Guilds |
                GatewayIntents.GuildMessages | GatewayIntents.MessageContent
                // GatewayIntents.All
        };
        
        builder.Services
            .AddSingleton(config)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<CommandService>()
            .AddSingleton<Settings>()
            .AddSingleton<Events>()
            .AddSingleton<Miscellaneous>();

        _builder = builder;
    }

    private DiscordSocketClient _client;
    private CommandService _commands;

    private Events _events;

    public async Task Run()
    {
        var app = _builder.Build();
        app.MapControllers();
        
        
        var serviceProvider = app.Services;
        
        _client = serviceProvider.GetRequiredService<DiscordSocketClient>();
        _commands = serviceProvider.GetRequiredService<CommandService>();
        var token = serviceProvider.GetRequiredService<Settings>().GetSettings().Token;

        _events = serviceProvider.GetRequiredService<Events>();

        _client.Log += _events.Log;
        _client.Ready += _events.OnReady;
        _client.MessageReceived += _events.OnMessageReceived;
        _client.UserJoined += _events.OnUserJoined;

        _client.ReactionAdded += _events.GetReactionMethod(true);
        _client.ReactionRemoved += _events.GetReactionMethod(false);
        
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
        await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: serviceProvider);


        app.Use(async (context, next) =>
        {
            if (context.Request.Host.Host != "localhost")
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            await next.Invoke();
        });

        await app.RunAsync();
    }
}