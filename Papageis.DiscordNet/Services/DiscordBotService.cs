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
    private readonly DiscordSocketClient Client;
    private readonly IBaseBotModule[] Modules;
    private readonly IBaseSlashCommand[] Commands;
    private readonly DiscordBotConfiguration Configuration;
    private readonly SlashCommandManagerService SlashCommandManager;

    public DiscordBotService(
        ILogger<DiscordBotService> logger,
        DiscordBotConfiguration configuration,
        IBaseBotModule[] modules,
        IBaseSlashCommand[] commands,
        DiscordSocketClient client,
        SlashCommandManagerService slashCommandManager)
    {
        Logger = logger;
        Modules = modules;
        Client = client;
        Commands = commands;
        Configuration = configuration;
        SlashCommandManager = slashCommandManager;
    }

    public async Task StartAsync()
    {
        Logger.LogInformation("Initializing DiscordBot");
        Client.Log += Log;
        Client.Ready += OnReady;
        Client.SlashCommandExecuted += SlashCommandManager.OnSlashCommandExecuted;

        try
        {
            foreach (var module in Modules)
                await module.InitializeAsync();
        }
        catch (Exception e)
        {
            Logger.LogError("An error occurred during Module initialization: {RegisterException}", e);
        }
        
        await Client.LoginAsync(TokenType.Bot, Configuration.Auth.BotToken);
        await Client.StartAsync();

        await Task.Delay(-1);
    }

    private async Task OnReady()
    {
        await Client.SetStatusAsync(UserStatus.Online);
        await Client.SetGameAsync("the Universe", "https://spielepapagei.de", ActivityType.Listening);

        if (Configuration.Settings.DevelopMode)
            Logger.LogInformation("Invite link: {invite}",
                $"https://discord.com/api/oauth2/authorize?client_id={Client.CurrentUser.Id}&permissions=1099511696391&scope=bot%20applications.commands");

        Logger.LogInformation("Login as {username}#{id}", Client.CurrentUser.Username,
            Client.CurrentUser.DiscriminatorValue);
        
        RegisterGlobalCommandsAsync();
    }

    public IBaseBotModule[] GetBaseBotModules()
    {
        return Modules;
    }

    public async Task UnregisterAsync(IBaseBotModule module)
    {
        await module.UnregisterAsync();
    }
    
    public async Task RegisterGlobalCommandsAsync()
    {
        try
        {
            foreach (var command in Commands)
            {
                command.GetName();
                await RegisterSlashCommandAsync(command);
                await Task.Delay(TimeSpan.FromMilliseconds(100));
            }
        }
        catch (Exception e)
        {
            Logger.LogError("An error occurred during Module Command Registration: {RegisterException}", e);
        }
    }
    
    public async Task RegisterSlashCommandAsync(IBaseSlashCommand command)
    {
        var builder = await command.RegisterAsync();

        await Client.CreateGlobalApplicationCommandAsync(builder.Build());
    }
    
    public async Task RegisterSlashCommandAsync(IGuildSlashCommand command)
    {
        var builder = await command.RegisterAsync();
        var guildId = await command.GuildId();

        var guild = Client.GetGuild(guildId);
        guild.CreateApplicationCommandAsync(builder.Build());
    }

    private Task Log(LogMessage message)
    {
        message.ToILogger(Logger);
        return Task.CompletedTask;
    }
}