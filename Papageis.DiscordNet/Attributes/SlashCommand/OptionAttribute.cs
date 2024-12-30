using Discord;

namespace Papageis.DiscordNet.Attributes.SlashCommand;

[AttributeUsage(AttributeTargets.Method)]
public class OptionAttribute : Attribute
{
    public readonly string Name;
    public readonly string Description;
    public readonly ApplicationCommandOptionType Type;
    public readonly bool? IsRequired;
    public readonly bool? IsDefault;
    public readonly bool IsAutocomplete;
    public readonly double? MinValue;
    public readonly double? MaxValue;
    public readonly List<ChannelType> ChannelTypes = null;
    
    public OptionAttribute(
        string name, 
        ApplicationCommandOptionType type,
        string description,
        bool? isRequired = null,
        bool? isDefault = null,
        bool isAutocomplete = false,
        double? minValue = null,
        double? maxValue = null,
        List<ChannelType> channelTypes = null
    )
    {
        Name = name;
        Description = description;
        Type = type;
        IsRequired = isRequired;
        IsDefault = isDefault;
        IsAutocomplete = isAutocomplete;
        MinValue = minValue;
        MaxValue = maxValue;
        ChannelTypes = channelTypes;
    }
}