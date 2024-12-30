using System.Reflection;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using MoonCore.PluginFramework.Extensions;
using Papageis.DiscordNet.Configuration;
using Papageis.DiscordNet.Module;
using Papageis.DiscordNet.Services;

namespace Papageis.DiscordNet.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddDiscordBot(this IServiceCollection collection, Action<DiscordBotConfiguration> onConfigure, Action<DiscordSocketConfig>? onConfigureSocket = null)
    {
        // Bot config
        var configuration = new DiscordBotConfiguration();
        onConfigure.Invoke(configuration);
        collection.AddSingleton(configuration);
        
        // Socket config
        var socketConfig = new DiscordSocketConfig()
        {
            GatewayIntents = GatewayIntents.None
        };
        
        onConfigureSocket?.Invoke(socketConfig);
        
        // Discord socket client
        var discordSocketClient = new DiscordSocketClient(socketConfig);
        collection.AddSingleton(discordSocketClient);
        
        // Main service
        collection.AddSingleton<DiscordBotService>();
        collection.AddSingleton<InteractionHandlerService>();

        //
        collection.AddInterfaces(interfaceConfiguration =>
        {
            interfaceConfiguration.AddAssemblies([typeof(InteractionHandlerService).Assembly]);
            interfaceConfiguration.AddAssemblies(configuration.ModuleAssemblies);
            interfaceConfiguration.AddInterface<IBaseBotModule>();
        });
    }
}