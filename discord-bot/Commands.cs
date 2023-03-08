using Discord.Commands;
using Discord.WebSocket;

namespace discordBot.Commands;

public class Commands : ModuleBase<SocketCommandContext>
{
    private readonly Miscellaneous _miscellaneous;
    private readonly ReactionChecker _reactionChecker;
    
    public Commands(Miscellaneous miscellaneous, ReactionChecker reactionChecker)
    {
        _miscellaneous = miscellaneous;
        _reactionChecker = reactionChecker;
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

    [Command("checkReactions")]
    public async Task CheckReactions()
    {
        var allowed = (Context.User as SocketGuildUser)!.Roles
            .Any(x => x.Id is 763704069310906368 or 1070750038977949746);
        if (!allowed) return;

        var added = await _reactionChecker.Run();
        ReplyAsync("Реакции проверены. Выдано ролей: " + added);
    }
}