using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Papageis.DiscordNet.Module;

namespace Papapageis.DiscordNet.Test.Modules;

public class MyCoolModule : IBaseBotModule
{
    private readonly DiscordSocketClient Client;
    private readonly CoolSharedService CoolSharedService;
    private readonly ILogger<MyCoolModule> Logger;

    public MyCoolModule(DiscordSocketClient client, CoolSharedService coolSharedService, ILogger<MyCoolModule> logger)
    {
        Client = client;
        CoolSharedService = coolSharedService;
        Logger = logger;
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task UnregisterAsync()
        => Task.CompletedTask;
}