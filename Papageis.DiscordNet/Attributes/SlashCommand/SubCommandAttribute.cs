using Discord;

namespace Papageis.DiscordNet.Attributes.SlashCommand;

[AttributeUsage(AttributeTargets.Method)]
public class SubCommandAttribute : Attribute
{
    public readonly string Name;
    public readonly string Description;
    public readonly ApplicationCommandOptionType Type = ApplicationCommandOptionType.SubCommand;
    
    public SubCommandAttribute(
        string name,
        string description = null
    )
    {
        Name = name;
        Description = description;
    }
}