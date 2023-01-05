using System.Globalization;
using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;
using Microsoft.AspNetCore.Mvc;
using myGreeterBot.Entities;

namespace myGreeterBot;

[ApiController]
public class Controller : ControllerBase
{
    private readonly DiscordSocketClient _client;

    public Controller(DiscordSocketClient client)
    {
        _client = client;
    }

    [HttpPost("embed")]
    public async Task<IActionResult> Embed([FromBody] EmbedEntity embedEntity)
    {
        // HttpRequestRewindExtensions.EnableBuffering(HttpContext.Request);
        //
        // HttpContext.Request.EnableBuffering();
        //
        // using var reader = new StreamReader(HttpContext.Request.BodyReader.AsStream());
        // // reader.BaseStream.Seek(0, SeekOrigin.Begin); 
        // var body = await reader.ReadToEndAsync();
        //
        //
        // Console.WriteLine(body);

        var channel = _client.GetChannel(1050814965004644413) as IMessageChannel;

        var embed = new EmbedBuilder();

        var color = Regex.Matches(embedEntity.Color, "[a-z|0-9]{2}", RegexOptions.Multiline)
            .Select(x => Int32.Parse(x.Value, NumberStyles.HexNumber)).ToArray();

        var embedFields = embedEntity.Fields.Select(field =>
            new EmbedFieldBuilder().WithName(field.Title).WithValue(field.Text).WithIsInline(field.Inline));

        embed.WithTitle(embedEntity.Title)
            .WithDescription(embedEntity.Description)
            .WithUrl(embedEntity.URL)
            .WithColor(color[0], color[1], color[2])
            .WithFields(embedFields);

        channel?.SendMessageAsync("", false, embed.Build());


        return Ok("O HI");
    }
}