using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Papageis.DiscordNet.Configuration;
using Papageis.DiscordNet.Extensions;
using Papageis.DiscordNet.Module;

namespace Papageis.DiscordNet.Services;

public class DiscordBotService
{
    private readonly ILogger<DiscordBotService> Logger;
    private readonly DiscordBotConfiguration Configuration;
    private readonly DiscordSocketClient Client;
    private readonly IBaseBotModule[] Modules;

    public DiscordBotService(
        ILogger<DiscordBotService> logger,
        DiscordBotConfiguration configuration,
        IBaseBotModule[] modules,
        DiscordSocketClient client)
    {
        Logger = logger;
        Configuration = configuration;
        Modules = modules;
        Client = client;
    }

    public async Task StartAsync()
    {
        if (!Configuration.Settings.Enable)
        {
            Logger.LogInformation("DiscordBot has been Disabled Skipping Startup.");
        }
        
        Logger.LogInformation("Initializing DiscordBot");
        Client.Log += Log;
        Client.Ready += OnReady;

        foreach (var module in Modules)
            await InitializeAsync(module);
        
        await Client.LoginAsync(TokenType.Bot, Configuration.Auth.BotToken);
        await Client.StartAsync();

        await Task.Delay(-1);
    }

    private async Task OnReady()
    {
        if (Configuration.Settings.DevelopMode)
            Logger.LogInformation("Invite link: {invite}",
                $"https://discord.com/api/oauth2/authorize?client_id={Client.CurrentUser.Id}&permissions=1099511696391&scope=bot%20applications.commands");

        Logger.LogInformation("Login as {username}#{id}", Client.CurrentUser.Username,
            Client.CurrentUser.DiscriminatorValue);
        
        /*
        var builder = new SlashCommandBuilder()
            .WithName("ping")
            .WithDescription("Ping Command");
        //Client.CreateGlobalApplicationCommandAsync(builder.Build());
        Client.GetGuild(1076531815998828635).CreateApplicationCommandAsync(builder.Build());
        */
        
    }

    public IBaseBotModule[] GetBaseBotModules()
    {
        return Modules;
    }

    public async Task InitializeAsync(IBaseBotModule module)
    {
        try
        {
            await module.InitializeAsync();
        }
        catch (Exception e)
        {
            Logger.LogError("An error has occurred while initializing the BotModules: {Exception}", e);
            throw;
        }
    }
    
    public async Task UnregisterAsync(IBaseBotModule module)
    {
        await module.UnregisterAsync();
    }

    private Task Log(LogMessage message)
    {
        message.ToILogger(Logger);
        return Task.CompletedTask;
    }
}