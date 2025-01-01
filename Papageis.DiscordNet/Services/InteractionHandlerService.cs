using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Papageis.DiscordNet.Interfaces;
using Papageis.DiscordNet.Models;

namespace Papageis.DiscordNet.Services;

public class InteractionHandlerService : IBaseBotModule
{
    private readonly ILogger<InteractionHandlerService> Logger;
    private readonly DiscordSocketClient Client;
    private readonly IServiceProvider ServiceProvider;
    private readonly InteractionInitializationService InteractionService;

    public InteractionHandlerService(
        ILogger<InteractionHandlerService> logger,
        DiscordSocketClient client,
        IServiceProvider serviceProvider, InteractionInitializationService initializationService)
    {
        Logger = logger;
        Client = client;
        ServiceProvider = serviceProvider;
        InteractionService = initializationService;
    }

    public async Task InitializeAsync()
    {
        Client.SlashCommandExecuted += OnSlashCommandExecuted;
    }

    public async Task UnregisterAsync()
    {
        Client.SlashCommandExecuted -= OnSlashCommandExecuted;
    }

    private async Task OnSlashCommandExecuted(SocketSlashCommand command)
    {
        try
        {
            var commandExecutor =
                InteractionService.GetSlashCommands().FirstOrDefault(x => x.Name == command.CommandName);
            if (commandExecutor == null)
            {
                await ReturnNotFound(command, "commandExecutor is null");
                return;
            }

            var commandModel = commandExecutor.Options;
            var optionsData = command.Data.Options;

            var interactionInstance = ServiceProvider.GetRequiredService(commandExecutor.InteractionClass) as InteractionContext<SocketSlashCommand>;
            if (interactionInstance != null && commandExecutor.MethodInfo != null)
            {
                var parameters = await LoadOptions(command, commandModel, optionsData);

                interactionInstance.Context = command;
                commandExecutor.MethodInfo.Invoke(interactionInstance, parameters);
                return;
            }

            foreach (var subOption in optionsData)
            {
                var discordSubOptionData = subOption;
                OptionsData? subCommand = new OptionsData();
                object[]? parameters = [];

                switch (subOption.Type)
                {
                    case ApplicationCommandOptionType.SubCommand:
                        subCommand = commandExecutor.Options.FirstOrDefault(x => x.Name == subOption.Name);
                        break;
                    case ApplicationCommandOptionType.SubCommandGroup:
                        var group = commandExecutor.Options.FirstOrDefault(x => x.Name == subOption.Name);
                        interactionInstance = ServiceProvider.GetRequiredService(group.InteractionClass) as InteractionContext<SocketSlashCommand>;
                        
                        var subSubOption = group.Options.FirstOrDefault(x => x.Name == subOption.Options.First().Name);
                        subCommand = group.Options.FirstOrDefault(x => x.Name == subSubOption.Name);
                        discordSubOptionData = subOption.Options.First();
                        break;
                    case ApplicationCommandOptionType.String:
                    case ApplicationCommandOptionType.Integer:
                    case ApplicationCommandOptionType.Boolean:
                    case ApplicationCommandOptionType.User:
                    case ApplicationCommandOptionType.Channel:
                    case ApplicationCommandOptionType.Role:
                    case ApplicationCommandOptionType.Mentionable:
                    case ApplicationCommandOptionType.Number:
                    case ApplicationCommandOptionType.Attachment:
                    default:
                        await ReturnNotFound(command, "Type out of Index");
                        return;
                }

                if (interactionInstance == null || subCommand == null || subCommand.MethodInfo == null) continue;
                var x = subCommand.Options ?? [];
                parameters = await LoadOptions(command, x, discordSubOptionData.Options);

                interactionInstance.Context = command;
                subCommand.MethodInfo.Invoke(interactionInstance, parameters);
                return;
            }

            await ReturnNotFound(command);
        }
        catch (Exception e)
        {
            await ReturnNotFound(command, e);
            throw;
        }
    }


    public async Task<object[]?> LoadOptions(SocketSlashCommand command, List<OptionsData> commandModel, IReadOnlyCollection<SocketSlashCommandDataOption>? optionsData)
    {
        List<object?> parameters = [];
        if (optionsData == null) return null;
        foreach (var option in commandModel)
        {
            var data = optionsData.FirstOrDefault(x => x.Name == option.Name);
            if (data == null)
            {
                await ReturnNotFound(command, "values are wrong or null.");
                return null;
            }

            if (option.IsRequired ?? false)
                if (data.Value == null)
                {
                    await ReturnNotFound(command, "value can not be null.");
                    return null;
                }

            switch (option.Type)
            {
                case ApplicationCommandOptionType.String:
                    var val0 = data.Value as string;
                    parameters.Add(val0 ?? null);
                    break;
                case ApplicationCommandOptionType.Integer:
                    var val1 = data.Value as int? ?? null;
                    parameters.Add(val1 ?? null);
                    break;
                case ApplicationCommandOptionType.Boolean:
                    var val2 = data.Value as bool? ?? null;
                    parameters.Add(val2 ?? null);
                    break;
                case ApplicationCommandOptionType.User:
                    var val3 = data.Value as IUser;
                    parameters.Add(val3 ?? null);
                    break;
                case ApplicationCommandOptionType.Channel:
                    var val4 = data.Value as IGuildChannel;
                    parameters.Add(val4 ?? null);
                    break;
                case ApplicationCommandOptionType.Role:
                    var val5 = data.Value as IRole;
                    parameters.Add(val5 ?? null);
                    break;
                case ApplicationCommandOptionType.Mentionable:
                    var val6 = data.Value as IMentionable;
                    parameters.Add(val6 ?? null);
                    break;
                case ApplicationCommandOptionType.Number:
                    var val7 = data.Value as double? ?? null;
                    parameters.Add(val7 ?? null);
                    break;
                case ApplicationCommandOptionType.Attachment:
                    var val8 = data.Value as IAttachment;
                    parameters.Add(val8);
                    break;

                case ApplicationCommandOptionType.SubCommand:
                case ApplicationCommandOptionType.SubCommandGroup:
                default:
                    await ReturnNotFound(command, "Type out of Index");
                    return null;
            }
        }
        return parameters.ToArray();
    }

    private async Task ReturnNotFound(SocketSlashCommand command, string? reason = null)
    {
        command.RespondAsync("You found a Black hole", ephemeral: true);
        Logger.LogWarning("SlashCommand with the name {CommandName} was unable to parse: {Reason}", command.CommandName, reason ?? "");
    }
    
    private async Task ReturnNotFound(SocketSlashCommand command, Exception? exception)
    {
        command.RespondAsync("You found a Black hole", ephemeral: true);
        Logger.LogWarning("SlashCommand with the name {CommandName} was unable to parse: {Exception}", command.CommandName, exception);
    }
}