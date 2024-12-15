using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Papageis.DiscordNet.Attributes;
using Papageis.DiscordNet.Module;

namespace Papageis.DiscordNet.Services;

public class SlashCommandManagerService
{
    private readonly ILogger<SlashCommandManagerService> Logger;
    private readonly DiscordSocketClient Client;
    private readonly IBaseSlashCommand[] CommandInterfaces;

    public SlashCommandManagerService(IBaseSlashCommand[] commandInterfaces, DiscordSocketClient client, ILogger<SlashCommandManagerService> logger)
    {
        Client = client;
        Logger = logger;
        CommandInterfaces = commandInterfaces;
    }
    
    public async Task OnSlashCommandExecuted(SocketSlashCommand command)
    {
        var commandName = command.CommandName;
        var iCommand = CommandInterfaces.FirstOrDefault(x => x.Name == commandName);

        var type = iCommand.GetType();
        var attributes= type.GetCustomAttributes(true);

        foreach (var attribute in attributes)
        {
            switch (attribute)
            {
                case IsBotAttribute x:
                    if(x.CanBeBot) continue;
                    if(command.User.IsBot) return;
                    continue;
                
                case IsDmChannelAttribute x:
                    if(x.CanBeDm) continue;
                    if(command.IsDMInteraction) return;
                    break;
            }
        }
        
        await iCommand.CommandExecuted(command);
    }
}