using Discord.Commands;
using Discord.WebSocket;

namespace discordBot.Commands;

public class Commands : ModuleBase<SocketCommandContext>
{
    private readonly Miscellaneous _miscellaneous;
    
    public Commands(Miscellaneous miscellaneous)
    {
        _miscellaneous = miscellaneous;
    }

    [Command("updateSettings")]
    public async Task UpdateSettings()
    {
        var allowed = (Context.User as SocketGuildUser)!.Roles
            .Any(x => x.Id is 763704069310906368 or 1070750038977949746);
        if (!allowed) return;

        await _miscellaneous.UpdateSettings();
        ReplyAsync("Настройки обновлены.");
    }
}