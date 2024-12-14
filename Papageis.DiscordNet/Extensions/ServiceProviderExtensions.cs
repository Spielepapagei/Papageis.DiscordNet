using Microsoft.Extensions.DependencyInjection;
using Papageis.DiscordNet.Services;

namespace Papageis.DiscordNet.Extensions;

public static class ServiceProviderExtensions
{
    public static async Task StartDiscordBot(this IServiceProvider provider, bool runAsync = false)
    {
        var discordBotService = provider.GetRequiredService<DiscordBotService>();

        if (runAsync)
            Task.Run(discordBotService.StartAsync);
        else
            await discordBotService.StartAsync();
    }
    
    public static async Task RegisterGlobalCommands(this IServiceProvider provider, bool runAsync = false)
    {
        var discordBotService = provider.GetRequiredService<DiscordBotService>();

        if (runAsync)
            Task.Run(discordBotService.RegisterGlobalCommandsAsync);
        else
            await discordBotService.RegisterGlobalCommandsAsync();
    }
}