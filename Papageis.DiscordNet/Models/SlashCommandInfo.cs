using System.Reflection;
using Discord;

namespace Papageis.DiscordNet.Models;

public class SlashCommandInfo
{
    public string Name { get; set; }
    public MethodInfo? MethodInfo { get; set; }
    public Type InteractionClass { get; set; }

    public List<SubCommandOptions> Options { get; set; } = new();
    
    public class SubCommandOptions
    {
        public MethodInfo? MethodInfo { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ApplicationCommandOptionType Type { get; set; }
        public bool? IsRequired { get; set; } = null;
        public bool? IsDefault { get; set; } = null;
        public bool IsAutocomplete { get; set; }
        public double? MinValue { get; set; } = null;
        public double? MaxValue { get; set; } = null;
        public List<ChannelType> ChannelTypes { get; set; } = null;
        public List<SubCommandOptions>? Options { get; set; } = null;
    }
}