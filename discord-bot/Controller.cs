using System.Globalization;
using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;
using discordBot.Data;
using Microsoft.AspNetCore.Mvc;
using myGreeterBot.Entities;

namespace myGreeterBot;

[ApiController]
public class Controller : ControllerBase
{
    private readonly DiscordSocketClient _client;
    private readonly Settings _settings;

    public Controller(DiscordSocketClient client, Settings settings)
    {
        _client = client;
        _settings = settings;
    }

    [HttpPost("embed")]
    public IActionResult Embed([FromBody] EmbedEntity embedEntity)
    {

        var settingsEntity = _settings._settingsEntity;
        var channel = _client.GetChannel(settingsEntity.NewsChannelId) as IMessageChannel;

        var embed = new EmbedBuilder();

        var color = Regex.Matches(embedEntity.Color, "[a-z|0-9]{2}", RegexOptions.Multiline)
            .Select(x => Int32.Parse(x.Value, NumberStyles.HexNumber)).ToArray();

        var embedFields = embedEntity.Fields.Select(field =>
            new EmbedFieldBuilder().WithName(field.Title).WithValue(field.Text).WithIsInline(field.Inline));

        var embedAuthor = new EmbedAuthorBuilder().WithName(embedEntity.Author.Name).WithUrl(embedEntity.Author.URL)
            .WithIconUrl(embedEntity.Author.Icon_URL);

        var embedFooter = new EmbedFooterBuilder().WithText(embedEntity.Footer.Text)
            .WithIconUrl(embedEntity.Footer.Icon_URL);

        embed.WithTitle(embedEntity.Title)
            .WithDescription(embedEntity.Description)
            .WithUrl(embedEntity.URL)
            .WithColor(color[0], color[1], color[2])
            .WithFields(embedFields)
            // .WithTimestamp();
            .WithImageUrl(embedEntity.Image.URL)
            .WithThumbnailUrl(embedEntity.Thumbnail.URL)
            .WithAuthor(embedAuthor)
            .WithFooter(embedFooter);

        channel?.SendMessageAsync("", false, embed.Build());
        
        return Ok("News sent");
    }
}