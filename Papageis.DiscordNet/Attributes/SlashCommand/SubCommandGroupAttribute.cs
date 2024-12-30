using Discord;

namespace Papageis.DiscordNet.Attributes.SlashCommand;

[AttributeUsage(AttributeTargets.Class)]
public class SubCommandGroupAttribute : Attribute
{
    public readonly Type GroupOf;
    public readonly string Name;
    public readonly string Description;
    public readonly ApplicationCommandOptionType Type = ApplicationCommandOptionType.SubCommandGroup;
    
    public SubCommandGroupAttribute(
        Type groupOf,
        string name,
        string description = null
    )
    {
        GroupOf = groupOf;
        Name = name;
        Description = description;
    }
}