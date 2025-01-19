using System.Reflection;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MoonCore.Extensions;
using MoonCore.Helpers;
using MoonCore.Services;
using Papageis.DiscordNet.Extensions;
using Papageis.DiscordNet.Services;
using Papapageis.DiscordNet.Test;
using Papapageis.DiscordNet.Test.Configuration;
using Papapageis.DiscordNet.Test.Modules;

Directory.CreateDirectory(PathBuilder.Dir("storage"));
var configService = new ConfigService<AppConfiguration>(PathBuilder.File("storage", "config.json"));

// Logging
var providers = LoggerBuildHelper.BuildFromConfiguration(configuration =>
{
    configuration.Console.Enable = true;
    configuration.Console.EnableAnsiMode = true;
    configuration.FileLogging.Enable = false;
});

var startupLoggerFactory = new LoggerFactory();
startupLoggerFactory.AddProviders(providers);

var serviceCollection = new ServiceCollection();

serviceCollection.AddSingleton(configService.Get());

serviceCollection.AddDiscordBot(configuration =>
{
    configuration.ModuleAssemblies.Add(Assembly.GetEntryAssembly()!);
    configuration.Auth.BotToken = configService.Get().BotToken;
    configuration.Settings.DevelopMode = true;
    configuration.Settings.EnableDebug = configService.Get().Settings.EnableDebug;
    configuration.Settings.Enable = configService.Get().Settings.Enable;

},
config =>
{
    config.GatewayIntents = GatewayIntents.None;
});

serviceCollection.AddLogging(builder =>
{
    builder.AddProviders(providers);
});

//Bot Testy
serviceCollection.AddSingleton<MyCoolCommand>();
var serviceProvider = serviceCollection.BuildServiceProvider();

await serviceProvider.StartDiscordBot();