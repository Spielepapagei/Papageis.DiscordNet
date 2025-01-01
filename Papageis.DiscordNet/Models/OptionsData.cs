using System.Reflection;
using Discord;

namespace Papageis.DiscordNet.Models;

public class OptionsData
{
    public MethodInfo? MethodInfo { get; set; }
    public Type? InteractionClass { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool UseLocalizedNaming { get; set; } = false;
    public ApplicationCommandOptionType Type { get; set; }
    public bool? IsRequired { get; set; } = null;
    public bool? IsDefault { get; set; } = null;
    public bool IsAutocomplete { get; set; }
    public double? MinValue { get; set; } = null;
    public double? MaxValue { get; set; } = null;
    public int? MinLength { get; set; } = null;
    public int? MaxLength { get; set; } = null;
    public List<ChannelType> ChannelTypes { get; set; }
    public List<ApplicationCommandOptionChoiceProperties>? Choices { get; set; } = null;
    public List<OptionsData>? Options { get; set; } = null;
}