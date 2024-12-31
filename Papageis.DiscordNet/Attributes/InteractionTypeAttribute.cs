using Discord;
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
    public readonly ApplicationIntegrationType IntegrationTypes;

    public InteractionTypeAttribute(
        ValidInteractionType type,
        string name,
        string description = null,
        bool useLocalizedNaming = false,
        bool nsfw = false,
        GuildPermission guildPermission = default,
        ApplicationIntegrationType integrationTypes = default)
    {
        Type = type;
        Name = name;
        Description = description;
        UseLocalizedNaming = useLocalizedNaming;
        Nsfw = nsfw;
        GuildPermission = guildPermission;
        IntegrationTypes = integrationTypes;
    }
}