using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Papageis.DiscordNet.Attributes;
using Papageis.DiscordNet.Attributes.SlashCommand;
using Papageis.DiscordNet.Enums;
using Papageis.DiscordNet.Services;

namespace Papapageis.DiscordNet.Test.Modules;

[InteractionType(ValidInteractionType.SlashCommand, "user", "a command")]
public class MyCoolCommand : InteractionContext<SocketSlashCommand>
{
    private readonly ILogger<MyCoolCommand> Logger;

    public MyCoolCommand(ILogger<MyCoolCommand> logger)
    {
        Logger = logger;
    }
    
    
    [SubCommandGroup(typeof(MyCoolCommand), "moderation", "moderation tools")]
    public class MyCoolGroup : InteractionContext<SocketSlashCommand>
    {
        
        [SubCommand("ban", "ban a user")]
        [SlashCommandOption(ApplicationCommandOptionType.User, "user", "select the user to ban", true)]
        public async Task MyCoolSubCommand([FromOptions]IUser user)
        {
            Context.RespondAsync($"Success from MySubCommand User: {user.Username}");
        }
        
    }
    
    [SubCommand( "info", "get info about an user")]
    [SlashCommandOption(ApplicationCommandOptionType.User, "user", "select the user to ban", true)]
    public async Task MySubCommand(IUser user)
    {
        Context.RespondAsync($"Success from MySubCommand User: {user.Username}");
    }
}