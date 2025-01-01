using System.Diagnostics;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Papageis.DiscordNet.Models;

namespace Papageis.DiscordNet.Services;

public class InteractionBuilderService
{
    public readonly ILogger<InteractionBuilderService> Logger;
    public readonly DiscordSocketClient Client;
    public readonly InteractionInitializationService InitializationService;

    public InteractionBuilderService(ILogger<InteractionBuilderService> logger, DiscordSocketClient client, InteractionInitializationService initializationService)
    {
        Logger = logger;
        Client = client;
        InitializationService = initializationService;
    }

    public async Task RegisterAllGlobalApplicationSlashCommands()
    {
        Logger.LogInformation("Registering all SlashCommand");
        var sw = new Stopwatch();
        sw.Start();
        var slashCommands = InitializationService.GetSlashCommands();
        List<SlashCommandBuilder> builders = [];

        foreach (var slashCommand in slashCommands)
        {
            builders.Add(await BuildSlashCommandInteraction(slashCommand));
        }
        
        var commandsOnDiscord = await Client.GetGlobalApplicationCommandsAsync();
        var commandsToRemove = commandsOnDiscord.ToList();
        
        foreach (var builder in builders)
        {
            var commandOnDiscord = commandsOnDiscord.FirstOrDefault(x => x.Name == builder.Name);
            if (commandOnDiscord != null) commandsToRemove.Remove(commandOnDiscord);
            
            await Client.CreateGlobalApplicationCommandAsync(builder.Build());
            Logger.LogInformation("+ '{CommandName}' SlashCommand", builder.Name);
            await Task.Delay(TimeSpan.FromMilliseconds(500));
        }

        foreach (var removeCommand in commandsToRemove)
        {
            removeCommand.DeleteAsync();
            Logger.LogInformation("- '{CommandName}' SlashCommand", removeCommand.Name);
        }
        
        sw.Stop();
        Logger.LogInformation("Done registered all SlashCommand in {Time}ms", sw.ElapsedMilliseconds);
    }
    
    private async Task<SlashCommandBuilder> BuildSlashCommandInteraction(SlashCommandInfo command)
    {
        try
        {
            var builder = new SlashCommandBuilder();

            builder.Name = command.Name;
            builder.Description = command.Description;
            if (command.UseLocalizedNaming)
            {
                //TODO: ImplementLocalizedNaming
            }

            builder.IsNsfw = command.Nsfw;
            builder.DefaultMemberPermissions = command.GuildPermission;
            builder.ContextTypes = command.ContextTypes;
            builder.IntegrationTypes = command.IntegrationTypes;

            var options = command.Options;

            foreach (var option in options)
            {
                builder.AddOption(await GenerateOptions(option));
            }

            return builder;
        }
        catch (Exception e)
        {
            Logger.LogError("An error has occurred while building an SlashCommand: {Exception}", e);
            throw;
        }
    }

    private async Task<SlashCommandOptionBuilder> GenerateOptions(SlashCommandInfo.OptionsData option)
    {
        try
        {
            var optionBuilder = new SlashCommandOptionBuilder
            {
                Type = option.Type,
                Name = option.Name,
                Description = option.Description,
                IsAutocomplete = option.IsAutocomplete,
                IsDefault = option.IsDefault,
                IsRequired = option.IsRequired,
                MaxValue = option.MaxValue,
                MinValue = option.MinValue,
                MaxLength = option.MaxLength,
                MinLength = option.MinLength,
                ChannelTypes = option.ChannelTypes
            };

            
            if (option.Options == null) return optionBuilder;
            foreach (var subOption in option.Options)
            {
                optionBuilder.AddOption(await GenerateOptions(subOption));
            }

            return optionBuilder;
        }
        catch (Exception e)
        {
            Logger.LogError("An error has occurred while building OptionData: {Exception}", e);
            throw;
        }
    }
}