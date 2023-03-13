using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using discordBot.Data;

namespace discordBot;

public class Events
{
    private readonly IServiceProvider _serviceProvider;

    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;

    private readonly Miscellaneous _miscellaneous;
    private readonly Settings _settings;

    private readonly ILogger<Events> _logger;


    public Events(DiscordSocketClient client, CommandService commands, Miscellaneous miscellaneous, Settings settings,
        IServiceProvider serviceProvider, ILogger<Events> logger)
    {
        _client = client;
        _commands = commands;
        _miscellaneous = miscellaneous;
        _settings = settings;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task OnMessageReceived(SocketMessage messageParam)
    {
        if (messageParam is not SocketUserMessage message) return;

        var argPos = 0;

        if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
            message.Author.IsBot)
            return;

        var context = new SocketCommandContext(_client, message);

        await _commands.ExecuteAsync(context: context, argPos: argPos, services: _serviceProvider);
    }

    public async Task OnReady() => _miscellaneous.CompareSettings();

    public async Task OnUserJoined(SocketGuildUser user)
    {
        var _settingsEntity = _settings._settingsEntity;

        var channel = _client.GetChannel(_settingsEntity.MentionChannelId) as IMessageChannel;
        var msg = await channel.SendMessageAsync(user.Mention);

        msg.DeleteAsync();
    }

    public Func<Cacheable<IUserMessage, ulong>, Cacheable<IMessageChannel, ulong>, SocketReaction, Task>
        GetReactionMethod(bool isAdded)
        => async (_, _, socketReaction) =>
        {
            var _settingsEntity = _settings._settingsEntity;

            var emoteAndRole = _settingsEntity.EmoteAndRole;
            var emoteId = socketReaction.Emote.ToString();

            if (socketReaction.MessageId != _settingsEntity.MessageId || socketReaction.User.Value.IsBot ||
                !emoteAndRole.Keys.Contains(emoteId)) return;

            var user = socketReaction.User.Value as IGuildUser;
            if (isAdded)
                user.AddRoleAsync(emoteAndRole[emoteId]);
            else
                user.RemoveRoleAsync(emoteAndRole[emoteId]);
        };

    public async Task Log(LogMessage log)
    {
        void LogMethod(string msg)
        {
            var lvl = log.Severity;
            if (lvl is LogSeverity.Debug or LogSeverity.Info)
            {
                _logger.LogInformation(msg);
                return;
            }

            _logger.LogWarning(msg);
        }

        var exp = log.Exception;
        LogMethod($"{log.Source} | {(exp == null ? log.Message : exp.ToString())}");
    }
}