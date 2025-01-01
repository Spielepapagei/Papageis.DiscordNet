using Discord;

namespace Papageis.DiscordNet.Attributes.SlashCommand;

[AttributeUsage(AttributeTargets.Method)]
public class SubCommandAttribute : Attribute
{
    public readonly string Name;
    public readonly string Description;
    public readonly bool UseLocalizedNaming;
    public readonly ApplicationCommandOptionType Type = ApplicationCommandOptionType.SubCommand;
    
    public SubCommandAttribute(
        string name,
        string description,
        bool useLocalizedNaming = false
    )
    {
        Name = name;
        Description = description;
        UseLocalizedNaming = useLocalizedNaming;
    }
}