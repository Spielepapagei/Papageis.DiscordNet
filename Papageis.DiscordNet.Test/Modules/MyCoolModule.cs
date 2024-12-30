using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Papageis.DiscordNet.Module;

namespace Papageis.DiscordNet.Test.Modules;

public class MyCoolModule : IBaseBotModule
{
    private readonly DiscordSocketClient Client;
    private readonly ILogger<MyCoolModule> Logger;

    public MyCoolModule(DiscordSocketClient client, ILogger<MyCoolModule> logger)
    {
        Client = client;
        Logger = logger;
    }

    public Task InitializeAsync()
        => Task.CompletedTask;

    public Task UnregisterAsync()
        => Task.CompletedTask;
}