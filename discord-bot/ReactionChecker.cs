using Discord;
using Discord.Rest;
using Discord.WebSocket;
using discordBot.Data;

namespace discordBot;

public class ReactionChecker
{
    private readonly SettingsEntity _settingsEntity;
    private readonly DiscordSocketClient _client;

    public ReactionChecker(Settings settings, DiscordSocketClient client)
    {
        _client = client;
        _settingsEntity = settings._settingsEntity;
    }

    public async Task<int> Run()
    {
        var channelId = _settingsEntity.MentionChannelId;
        var msgId = _settingsEntity.MessageId;

        var channel = _client.GetChannel(channelId) as SocketTextChannel;
        var msg = await channel.GetMessageAsync(msgId) as RestUserMessage;

        var addRolesCount = 0;
        foreach (var reactionsKey in msg.Reactions.Keys)
        {
            var users = await msg.GetReactionUsersAsync(reactionsKey, limit: 100).FlattenAsync();

            var emote = reactionsKey as Emote;
            var roleId = _settingsEntity.EmoteAndRole[$"<:{emote.Name}:{emote.Id}>"];
            
            foreach (var user in users)
            {
                var socketGuildUser = channel.GetUser(user.Id);
                if (socketGuildUser == null) continue;

                var doesUserHaveThisRole = socketGuildUser.Roles.Any(x => x.Id == roleId);
                if (doesUserHaveThisRole || user.IsBot) continue;
                
                Console.WriteLine($"ri: {roleId} | ui: {user.Id}, un: {user.Username}");
                await socketGuildUser.AddRoleAsync(roleId);
                addRolesCount++;
            }
        }

        return addRolesCount;
    }
}