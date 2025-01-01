using System.Reflection;
using Microsoft.Extensions.Logging;
using Papageis.DiscordNet.Attributes;
using Papageis.DiscordNet.Attributes.SlashCommand;
using Papageis.DiscordNet.Configuration;
using Papageis.DiscordNet.Enums;
using Papageis.DiscordNet.Models;

namespace Papageis.DiscordNet.Services;

public class InteractionInitializationService
{
    #region Initializeation
    private readonly ILogger<InteractionInitializationService> Logger;
    private readonly DiscordBotConfiguration Configuration;

    public InteractionInitializationService(
        ILogger<InteractionInitializationService> logger,
        DiscordBotConfiguration configuration
    )
    {
        Logger = logger;
        Configuration = configuration;
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
                                InteractionClass = interaction,
                                Name = interactionType.Name,
                                Description = interactionType.Description,
                                UseLocalizedNaming = interactionType.UseLocalizedNaming,
                                Nsfw = interactionType.Nsfw,
                                ContextTypes = interactionType.ContextTypes,
                                IntegrationTypes = interactionType.IntegrationTypes,
                                GuildPermission = interactionType.GuildPermission
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

    private async Task<SlashCommandInfo.OptionsData> GetSlashCommandGroupInfoFromMethod(Type type)
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
                //public List<ApplicationCommandOptionChoiceProperties> Choices;
                //Choices = optionAttribute.Choices
            });
        }
            
        return list;
    }

    public List<SlashCommandInfo> GetSlashCommands()
    {
        return SlashCommands;
    }
}