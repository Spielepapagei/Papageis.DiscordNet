using Discord;

namespace Papageis.DiscordNet.Attributes.SlashCommand;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class SlashCommandOptionAttribute : Attribute
{
    public readonly ApplicationCommandOptionType Type;
    public readonly string Name;
    public readonly string Description;
    public readonly bool IsRequired;
    public readonly bool IsDefault;
    public readonly bool IsAutocomplete;
    public readonly bool UseLocalizedNaming;
    public readonly double? MinValue;
    public readonly double? MaxValue;
    public readonly int? MinLength;
    public readonly int? MaxLength;
    public readonly List<ChannelType> ChannelTypes;
    
    public SlashCommandOptionAttribute(
        ApplicationCommandOptionType type,
        string name, 
        string description,
        bool isRequired = false,
        bool isDefault = false,
        bool isAutocomplete = false,
        bool useLocalizedNaming = false,
        double minValue = 0,
        double maxValue = 0,
        int minLength = 0,
        int maxLength = 0,
        ChannelType[] channelTypes = null
    )
    {
        Type = type;
        Name = name;
        Description = description;
        IsRequired = isRequired;
        IsDefault = isDefault;
        IsAutocomplete = isAutocomplete;
        UseLocalizedNaming = useLocalizedNaming;
        MinValue = minValue == 0 ? null : minValue;
        MaxValue = maxValue == 0 ? null : maxValue;
        MinLength = minLength == 0 ? null : minLength;
        MaxLength = maxLength == 0 ? null : maxLength;
        ChannelTypes = channelTypes == null ? null : channelTypes.ToList();
    }
}