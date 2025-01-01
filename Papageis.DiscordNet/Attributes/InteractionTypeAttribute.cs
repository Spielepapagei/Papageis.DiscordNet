using Discord;
using Discord.Commands;
using Papageis.DiscordNet.Enums;

namespace Papageis.DiscordNet.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class InteractionTypeAttribute : Attribute
{
    public readonly ValidInteractionType Type;
    public readonly string Name;
    public readonly string Description;
    public readonly bool UseLocalizedNaming;
    public readonly bool Nsfw;
    public readonly GuildPermission GuildPermission;
    public HashSet<InteractionContextType>? ContextTypes;
    public HashSet<ApplicationIntegrationType>? IntegrationTypes;

    public InteractionTypeAttribute(
        ValidInteractionType type,
        string name,
        string description,
        bool useLocalizedNaming = false,
        bool nsfw = false,
        GuildPermission guildPermission = default,
        InteractionContextType[] contextTypes = null,
        ApplicationIntegrationType[] integrationTypes = null)
    {
        Type = type;
        Name = name;
        Description = description;
        UseLocalizedNaming = useLocalizedNaming;
        Nsfw = nsfw;
        GuildPermission = guildPermission;
        ContextTypesToHashSet(contextTypes);
        IntegrationTypesToHashSet(integrationTypes);
    }
    
    private void ContextTypesToHashSet(InteractionContextType[]? contextTypes)
    {
        ContextTypes = contextTypes is not null 
            ? new HashSet<InteractionContextType>(contextTypes) 
            : null;
    }
    
    private void IntegrationTypesToHashSet(ApplicationIntegrationType[]? integrationTypes)
    {
        IntegrationTypes = integrationTypes is not null 
            ? new HashSet<ApplicationIntegrationType>(integrationTypes) 
            : null;
    }
}