using System.Reflection;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Papageis.DiscordNet.Attributes;
using Papageis.DiscordNet.Attributes.SlashCommand;
using Papageis.DiscordNet.Configuration;
using Papageis.DiscordNet.Models;
using Papageis.DiscordNet.Module;

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
                    
                    switch (interactionType.GetInteractionType())
                    {
                        #region InitSlashCommand
                        case InteractionType.SlashCommand:
                            var applicationCommandModel = new SlashCommandInfo
                            {
                                Name = interactionType.GetName(),
                                InteractionClass = interaction
                            };
                            
                            //Check if it is a command without nesting.
                            var entryPoint = interaction.GetMethods()
                                .FirstOrDefault(x => x.GetCustomAttribute<SlashCommandAttribute>() != null);
                            
                            if (entryPoint != null)
                            {
                                applicationCommandModel.MethodInfo = entryPoint;
                                
                                Logger.LogInformation("Command here!");
                                
                                //TODO: Add option handling
                                SlashCommands.Add(applicationCommandModel);
                                break;
                            }

                            //Nesting logic
                            #region SubCommandGroups
                            var subCommandGroups = interaction.GetNestedTypes()
                                    .Where(x => x.GetCustomAttribute<SubCommandGroupAttribute>() != null);

                            foreach (var group in subCommandGroups)
                            {
                                var groupAttribute = group.GetCustomAttribute<SubCommandGroupAttribute>();

                                var subCommandsInGroup = group.GetMethods()
                                    .Where(x => x.GetCustomAttribute<SubCommandAttribute>() != null);

                                var subOptions = new List<SlashCommandInfo.SubCommandOptions>();
                                foreach(var x in subCommandsInGroup)
                                {
                                    var attribute = x.GetCustomAttribute<SubCommandAttribute>();
                                    
                                    subOptions.Add(new()
                                    {
                                        MethodInfo = x,
                                        Name = attribute.Name,
                                        Description = attribute.Description,
                                        Type = attribute.Type
                                    });
                                }
                                
                                applicationCommandModel.Options.Add(new()
                                {
                                    Name = groupAttribute.Name,
                                    Description = groupAttribute.Description,
                                    Type = groupAttribute.Type,
                                    Options = subOptions
                                });
                            }
                            #endregion
                            
                            var subCommands = interaction.GetMethods()
                                .Where(x => x.GetCustomAttribute<SubCommandAttribute>() != null);

                            foreach (var subCommand in subCommands)
                            {
                                var subCommandAttribute = subCommand.GetCustomAttribute<SubCommandAttribute>();
                                
                                applicationCommandModel.Options.Add(new()
                                {
                                    MethodInfo = subCommand,
                                    Name = subCommandAttribute.Name,
                                    Description = subCommandAttribute.Description,
                                    Type = subCommandAttribute.Type
                                });
                            }
                            
                            SlashCommands.Add(applicationCommandModel);
                            break;
                        #endregion
                        
                        default:
                            Logger.LogInformation("InteractionType '{Type}' is not Supported", interactionType.GetInteractionType().ToString());
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
        var instance =
            ServiceProvider.GetRequiredService(commandExecutor.InteractionClass) as InteractionContext<SocketSlashCommand>;

        instance.Context = command;

        commandExecutor.MethodInfo.Invoke(instance, []);
    }
}