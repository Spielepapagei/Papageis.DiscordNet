using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Papageis.DiscordNet.Attributes;
using Papageis.DiscordNet.Attributes.SlashCommand;
using Papageis.DiscordNet.Services;

namespace Papageis.DiscordNet.Test.Modules;

[InteractionType("ping", InteractionType.SlashCommand)]
public class MyCoolCommand : InteractionContext<SocketSlashCommand>
{
    private readonly ILogger<MyCoolCommand> Logger;

    public MyCoolCommand(ILogger<MyCoolCommand> logger)
    {
        Logger = logger;
    }
    
    
    [SubCommandGroup(typeof(MyCoolCommand), "get")]
    public class MyCoolGroup : InteractionContext<SocketSlashCommand>
    {
        
        [SubCommand("ping")]
        public async Task MyCoolSubCommand()
        {
            Context.RespondAsync("Success from MyCoolSubCommand");
        }
        
    }
    
    [SubCommand( "sub-command")]
    public async Task MySubCommand()
    {
        Context.RespondAsync("Success from MySubCommand");
    }
}