using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using discordBot.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace discordBot;

public class BotRunner
{

    private readonly IServiceProvider _serviceProvider;
    
    public BotRunner()
    {
        _serviceProvider = CreateProvider();
    }

    private IServiceProvider CreateProvider()
    {
        var config = new DiscordSocketConfig()
        {
            GatewayIntents = GatewayIntents.All
        };
        _client = new DiscordSocketClient(config);
        _interactionService = new InteractionService(_client);

        var collection = new ServiceCollection()
            // .AddSingleton(config)
            .AddSingleton(_client)
            .AddSingleton<CommandService>()
            .AddSingleton<Settings>()
            .AddSingleton<Events>()
            .AddSingleton<Miscellaneous>()
            .AddSingleton(_interactionService)
            .AddSingleton<InteractionModule>();
        collection.AddControllers();
        
        return collection.BuildServiceProvider();
    }

    private DiscordSocketClient _client;
    private CommandService _commands;
    private InteractionService _interactionService;

    private Events _events;

    public async Task Run(WebApplicationBuilder builder)
    {
        _client = _serviceProvider.GetRequiredService<DiscordSocketClient>();
        _commands = _serviceProvider.GetRequiredService<CommandService>();
        var token = _serviceProvider.GetRequiredService<Settings>().GetSettings().Token;

        _events = _serviceProvider.GetRequiredService<Events>();
        
        _client.Log += _events.Log;
        _client.Ready += _events.OnReady;
        
        _client.MessageReceived += _events.OnMessageReceived;
        
        // _client.Handle

        // _client.UserJoined += _events.OnUserJoined;
        //
        // _client.ReactionAdded += _events.GetReactionMethod(true);
        // _client.ReactionRemoved += _events.GetReactionMethod(false);
        
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);

        _interactionService = _serviceProvider.GetRequiredService<InteractionService>();
        
        _client.InteractionCreated += async (x) =>
        {
            Console.WriteLine("??");
            var ctx = new SocketInteractionContext(_client, x);
            await _interactionService.ExecuteCommandAsync(ctx, _serviceProvider);
        };
        _client.SlashCommandExecuted  += async (x) =>
        {
            Console.WriteLine("slashCmd");
        };
        
        // JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        builder.Services.AddControllers();

        // builder.Services.AddAuthentication(config =>
        //     {
        //         config.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        //         config.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        //     })
        //     .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
        //     .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
        //     {
        //         options.Authority = "https://localhost:7274";
        //         options.ClientId = "platformnet6";
        //         options.ClientSecret = "123456789";
        //         options.ResponseType = "code";
        //         options.CallbackPath = "/signin-oidc";
        //         options.SaveTokens = true;
        //         options.TokenValidationParameters = new TokenValidationParameters
        //         {
        //             ValidateIssuerSigningKey = false,
        //             SignatureValidator = delegate(string token, TokenValidationParameters validationParameters)
        //             {
        //                 var jwt = new JwtSecurityToken(token);
        //                 return jwt;
        //             },
        //         };
        //     });

        var app = builder.Build();
        app.MapControllers();
        
        // app.UseAuthentication();
        // app.UseAuthorization();
        await app.RunAsync();


        // await Task.Delay(-1);
    }
}