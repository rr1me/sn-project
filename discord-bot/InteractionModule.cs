using Discord;
using Discord.Commands;
using Discord.Interactions;

namespace discordBot;

public class InteractionModule : InteractionModuleBase<SocketInteractionContext>
{
    [Command("whut")]
    public async Task upd(IUser user)
    {
        Console.WriteLine("yo");
        try
        {
            RespondAsync("YO MA MAAAAAAAAAAAAAAAAAAAAAN");
            ReplyAsync("?че");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}