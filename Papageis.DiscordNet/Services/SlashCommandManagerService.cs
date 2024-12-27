using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Papageis.DiscordNet.Attributes;
using Papageis.DiscordNet.Module;

namespace Papageis.DiscordNet.Services;

public class SlashCommandManagerService
{
    private readonly ILogger<SlashCommandManagerService> Logger;
    private readonly DiscordSocketClient Client;
    private readonly IBaseSlashCommand[] SlashCommandInterfaces;
    private readonly IBaseMessageCommand[] MessageCommandInterfaces;
    private readonly IBaseUserCommand[] UserCommandInterfaces;

    public SlashCommandManagerService(IBaseSlashCommand[] slashCommandInterfaces, DiscordSocketClient client, ILogger<SlashCommandManagerService> logger)
    {
        Client = client;
        Logger = logger;
        SlashCommandInterfaces = slashCommandInterfaces;
    }
    
    public async Task OnSlashCommandExecuted(SocketSlashCommand command)
    {
        var commandName = command.CommandName;
        var iCommand = SlashCommandInterfaces.FirstOrDefault(x => x.Name == commandName);

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

    public async Task OnMessageCommandExecuted(SocketMessageCommand command)
    {
        var commandName = command.CommandName;
        var iCommand = MessageCommandInterfaces.FirstOrDefault(x => x.Name == commandName);

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

    public async Task OnuserCommandExecuted(SocketUserCommand command)
    {
        var commandName = command.CommandName;
        var iCommand = UserCommandInterfaces.FirstOrDefault(x => x.Name == commandName);

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