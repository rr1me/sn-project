using Discord;
using Discord.WebSocket;
using discordBot.Data;

namespace discordBot;

public class Miscellaneous
{

    private readonly DiscordSocketClient _client;
    private readonly Settings _settings;
    private SettingsEntity _settingsEntity;

    public Miscellaneous(DiscordSocketClient client, Settings settings)
    {
        _client = client;
        _settings = settings;
        _settingsEntity = settings._settingsEntity;
    }

    public async Task UpdateSettings()
    {
        _settingsEntity = _settings.GetSettings();
        CompareSettings();
    }
    
    public async Task CompareSettings()
    {
        var channel = _client.GetChannel(_settingsEntity.MentionChannelId) as IMessageChannel;
        var messageTask = channel.GetMessageAsync(_settingsEntity.MessageId);

        IMessage message;
        if (messageTask.IsFaulted)
            message = await SendNewMessage(channel);
        else
        {
            try
            {
                message = await CheckMessage(await messageTask);
            }
            catch
            {
                message = await SendNewMessage(channel);
            }
        }

        CheckReactions(message);
    }

    private async Task<IMessage> CheckMessage(IMessage message)
    {
        var channel = message.Channel;
        var botId = _client.CurrentUser.Id;

        if (message.Author.Id != botId)
            return await SendNewMessage(channel);

        var messageText = _settingsEntity.MessageText;

        var embed = message.Embeds.First();
        var roleField = CreateRoleField();
        
        if (embed.Description != messageText || embed.Fields[0].Value != roleField)
            return await channel.ModifyMessageAsync(message.Id, properties => properties.Embed = CreateEmbed());

        return message;
    }
    
    private async Task<IMessage> SendNewMessage(IMessageChannel channel)
    {
        var message = await channel.SendMessageAsync("", false, CreateEmbed());

        _settingsEntity.MessageId = message.Id;
        _settings.SaveSettings(_settingsEntity);
        return message;
    }

    private Embed CreateEmbed()
    {
        var messageTitle = _settingsEntity.MessageTitle;
        var messageText = _settingsEntity.MessageText;
        var messageFooter = _settingsEntity.MessageFooter;

        var embed = new EmbedBuilder();

        const string iconUrl = "https://cdn.discordapp.com/attachments/1043312162023678002/1073617565072506921/bf481fe2fbb7afee.png";
        var embedAuthor = new EmbedAuthorBuilder().WithName("SmollNet").WithIconUrl(iconUrl);

        var embedField = new EmbedFieldBuilder().WithName("Доступные роли").WithValue(CreateRoleField());

        embed.WithTitle(messageTitle)
            .WithDescription(messageText)
            .WithUrl("http://smoll.net")
            .WithColor(Color.Blue)
            .WithThumbnailUrl(iconUrl)
            .WithAuthor(embedAuthor)
            .WithFooter(messageFooter)
            .WithFields(embedField);

        return embed.Build();
    }

    private string CreateRoleField()
    {
        var guild = _client.GetGuild(_settingsEntity.GuildId);
        var roles = _settingsEntity.EmoteAndRole;
        
        var roleField = "";
        foreach (var (key, value) in roles)
        {
            roleField = roleField + key + ": " + guild.GetRole(value).Name + "\n";
        }

        return roleField;
    }

    private async Task CheckReactions(IMessage message)
    {
        var reactions = message.Reactions;
        var formalReactions = _settingsEntity.EmoteAndRole.Keys;

        if (reactions.Count > formalReactions.Count)
        {
            foreach (var actualReaction in reactions.Keys)
            {
                if (!formalReactions.Contains(actualReaction.ToString()))
                    await message.RemoveAllReactionsForEmoteAsync(actualReaction);
            }
        }
        foreach (var emoteUnicode in formalReactions)
        {
            var reactionKeys = reactions.Keys.Select(x=>x.ToString());

            if (reactions.Count != 0 && reactionKeys.Contains(emoteUnicode)) continue;
            
            if (Emote.TryParse(emoteUnicode, out var emote))
                await message.AddReactionAsync(emote);
            else
                await message.AddReactionAsync(new Emoji(emoteUnicode));
        }
    }
}