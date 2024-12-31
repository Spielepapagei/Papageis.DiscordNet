using System.Reflection;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Papageis.DiscordNet.Attributes;
using Papageis.DiscordNet.Attributes.SlashCommand;
using Papageis.DiscordNet.Configuration;
using Papageis.DiscordNet.Enums;
using Papageis.DiscordNet.Interfaces;
using Papageis.DiscordNet.Models;

namespace Papageis.DiscordNet.Services;

public class InteractionHandlerService : IBaseBotModule
{
    #region Initializeation
    private readonly ILogger<InteractionHandlerService> Logger;
    private readonly DiscordSocketClient Client;
    private readonly DiscordBotConfiguration Configuration;
    private readonly IServiceProvider ServiceProvider;

    public InteractionHandlerService(
        ILogger<InteractionHandlerService> logger,
        DiscordSocketClient client,
        DiscordBotConfiguration configuration,
        IServiceProvider serviceProvider
    )
    {
        Logger = logger;
        Client = client;
        Configuration = configuration;
        ServiceProvider = serviceProvider;
    }

    public async Task InitializeAsync()
    {
        Client.SlashCommandExecuted += OnSlashCommandExecuted;
    }

    public async Task UnregisterAsync()
    {
        Client.SlashCommandExecuted -= OnSlashCommandExecuted;
    }
    #endregion
    
    public async Task InitializeInteractionHandler()
    {
        try
        {
            #region Gets all classes that Implement the InteractionContext
            foreach (var interactions in
                     Configuration.ModuleAssemblies.Select(assembly => assembly.ExportedTypes.Where(
                         x => x.BaseType != null &&
                              x.BaseType.IsGenericType &&
                              x.BaseType.GetGenericTypeDefinition() == typeof(InteractionContext<>)
                     )))
            #endregion
            {
                foreach (var interaction in interactions)
                {
                    var interactionType = interaction.GetCustomAttribute(typeof(InteractionTypeAttribute), false) as InteractionTypeAttribute;
                    if(interactionType == null) continue;
                    
                    switch (interactionType.Type)
                    {
                        #region InitSlashCommand
                        case ValidInteractionType.SlashCommand:
                            var applicationCommandModel = new SlashCommandInfo
                            {
                                Name = interactionType.Name,
                                InteractionClass = interaction
                            };
                            
                            //Check if it is a command without nesting.
                            var entryPoint = interaction.GetMethods()
                                .FirstOrDefault(x => x.GetCustomAttribute<SlashCommandAttribute>() != null);
                            
                            if (entryPoint != null)
                            {
                                applicationCommandModel.MethodInfo = entryPoint;
                                applicationCommandModel.Options = await GetOptionsFromMethod(entryPoint);
                                
                                SlashCommands.Add(applicationCommandModel);
                                break;
                            }

                            //Nesting logic
                            #region SubCommandGroups
                            var subCommandGroups = interaction.GetNestedTypes()
                                    .Where(x => x.GetCustomAttribute<SubCommandGroupAttribute>() != null);

                            foreach (var group in subCommandGroups)
                            {
                                applicationCommandModel.Options.Add(await GetSlashCommandGroupInfoFromMethod(group));
                            }
                            #endregion

                            #region SubCommands
                            var subCommands = interaction.GetMethods()
                                .Where(x => x.GetCustomAttribute<SubCommandAttribute>() != null);

                            foreach (var subCommand in subCommands)
                            {
                                applicationCommandModel.Options.Add(await GetSlashCommandInfoFromMethod(subCommand));
                            }
                            #endregion
                            
                            SlashCommands.Add(applicationCommandModel);
                            break;
                        #endregion
                        
                        default:
                            Logger.LogInformation("InteractionType '{Type}' is not Supported", interactionType.Type.ToString());
                            continue;
                    }
                }
            }
            
            Logger.LogInformation("Done!");
            
        }
        catch (Exception e)
        {
            Logger.LogError("An error has occurred while initializing the Interactions: {Exception}", e);
            throw;
        }
    }

    private List<SlashCommandInfo> SlashCommands = new();

    public async Task OnSlashCommandExecuted(SocketSlashCommand command)
    {
        var commandExecutor = SlashCommands.FirstOrDefault(x => x.Name == command.CommandName);
        var interactionInstance = ServiceProvider.GetRequiredService(commandExecutor.InteractionClass) as InteractionContext<SocketSlashCommand>;

        interactionInstance.Context = command;

        commandExecutor.MethodInfo.Invoke(interactionInstance, []);
    }

    public async Task<SlashCommandInfo.OptionsData> GetSlashCommandGroupInfoFromMethod(Type type)
    {
        var optionAttribute = type.GetCustomAttribute<SubCommandGroupAttribute>();
        var subCommandsInGroup = type.GetMethods()
            .Where(x => x.GetCustomAttribute<SubCommandAttribute>() != null);

        var subCommands = new List<SlashCommandInfo.OptionsData>();
        foreach (var subCommand in subCommandsInGroup)
        {
            subCommands.Add(await GetSlashCommandInfoFromMethod(subCommand));
        }
        
        return new()
        {
            GroupClass = type,
            Type = optionAttribute.Type,
            Name = optionAttribute.Name,
            UseLocalizedNaming = optionAttribute.UseLocalizedNaming,
            Description = optionAttribute.Description,
            Options = subCommands
        };
    }

    private async Task<SlashCommandInfo.OptionsData> GetSlashCommandInfoFromMethod(MethodInfo methodInfo)
    {
        var optionAttribute = methodInfo.GetCustomAttribute<SubCommandAttribute>();
        var commandOptions = await GetOptionsFromMethod(methodInfo);
        
        return new SlashCommandInfo.OptionsData
        {
            MethodInfo = methodInfo,
            Type = optionAttribute.Type,
            Name = optionAttribute.Name,
            UseLocalizedNaming = optionAttribute.UseLocalizedNaming,
            Description = optionAttribute.Description,
            Options = commandOptions
        };
    }

    private async Task<List<SlashCommandInfo.OptionsData>> GetOptionsFromMethod(MethodInfo methodInfo)
    {
        var list = new List<SlashCommandInfo.OptionsData>();
        var optionAttributes = methodInfo.GetCustomAttributes<SlashCommandOptionAttribute>();

        foreach (var optionAttribute in optionAttributes)
        {
            list.Add(new SlashCommandInfo.OptionsData
            {
                MethodInfo = methodInfo,
                Type = optionAttribute.Type,
                Name = optionAttribute.Name,
                UseLocalizedNaming = optionAttribute.UseLocalizedNaming,
                Description = optionAttribute.Description,
                IsAutocomplete = optionAttribute.IsAutocomplete,
                IsDefault = optionAttribute.IsDefault,
                IsRequired = optionAttribute.IsRequired,
                MaxValue = optionAttribute.MaxValue,
                MinValue = optionAttribute.MinValue,
                MaxLength = optionAttribute.MaxLength,
                MinLength = optionAttribute.MinLength,
                ChannelTypes = optionAttribute.ChannelTypes
                
                //TODO:Implement Choices back from parameters
                //public readonly List<ApplicationCommandOptionChoiceProperties> Choices;
                //Choices = optionAttribute.Choices
            });
        }
            
        return list;
    }
}