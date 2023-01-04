using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using discordBot.Data;

namespace discordBot;

public class BotRunner
{

    // private readonly IServiceProvider _serviceProvider;

    // private readonly IServiceCollection _serviceCollection;

    private readonly WebApplicationBuilder _builder;
    
    public BotRunner(WebApplicationBuilder builder)
    {
        var config = new DiscordSocketConfig()
        {
            GatewayIntents =
                // GatewayIntents.GuildMessageReactions |
                // GatewayIntents.GuildMembers |
                // GatewayIntents.GuildPresences |
                // GatewayIntents.Guilds |
                // GatewayIntents.GuildMessages | GatewayIntents.MessageContent
                GatewayIntents.All
        };
        
        builder.Services
            .AddSingleton(config)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<CommandService>()
            .AddSingleton<Settings>()
            .AddSingleton<Events>()
            .AddSingleton<Miscellaneous>();

        _builder = builder;

        // _serviceProvider = CreateProvider();

        // var app = builder.Build();
        //
        // _serviceProvider = app.Services;
    }

    // private IServiceProvider CreateProvider()
    // {
    //     var config = new DiscordSocketConfig()
    //     {
    //         GatewayIntents =
    //             GatewayIntents.GuildMessageReactions |
    //             GatewayIntents.GuildMembers |
    //             GatewayIntents.GuildPresences |
    //             GatewayIntents.Guilds |
    //             GatewayIntents.GuildMessages | GatewayIntents.MessageContent
    //     };
    //
    //     var collection = new ServiceCollection()
    //         .AddSingleton(config)
    //         .AddSingleton<DiscordSocketClient>()
    //         .AddSingleton<CommandService>()
    //         .AddSingleton<Settings>()
    //         .AddSingleton<Events>()
    //         .AddSingleton<Miscellaneous>();
    //     
    //     return collection.BuildServiceProvider();
    // }

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

        await app.RunAsync();
        // await Task.Delay(-1);
    }
}