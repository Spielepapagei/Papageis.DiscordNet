using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Papageis.DiscordNet.Models;

namespace Papageis.DiscordNet.Services;

public class InteractionBuilderService
{
    public readonly ILogger<InteractionBuilderService> Logger;
    public readonly DiscordSocketClient Client;

    public InteractionBuilderService(ILogger<InteractionBuilderService> logger, DiscordSocketClient client)
    {
        Logger = logger;
        Client = client;
    }

    public async Task BuildSlashCommandInteraction(SlashCommandInfo command)
    {
        
    }
}