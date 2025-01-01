using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Papageis.DiscordNet.Attributes;
using Papageis.DiscordNet.Attributes.SlashCommand;
using Papageis.DiscordNet.Enums;
using Papageis.DiscordNet.Services;

namespace Papapageis.DiscordNet.Test.Modules;

[InteractionType(ValidInteractionType.SlashCommand, "habeldabelduldelda", "idk")]
public class MyCoolSlashCommand : InteractionContext<SocketSlashCommand>
{
    private readonly ILogger<MyCoolSlashCommand> Logger;

    public MyCoolSlashCommand(ILogger<MyCoolSlashCommand> logger)
    {
        Logger = logger;
    }
    
    [SlashCommand]
    [SlashCommandOption(ApplicationCommandOptionType.String, "message", "Send a Message to X")]
    public async Task ExecuteCommand(string message, IUser user, IMentionable mentionable)
    {
        Context.RespondAsync($"Hello from {user.Username} side: {message} @{mentionable.Mention}", ephemeral: true);
    }
    
}