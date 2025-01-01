using System.Reflection;
using Discord;

namespace Papageis.DiscordNet.Models;

public class SlashCommandInfo
{
    public MethodInfo? MethodInfo { get; set; }
    public Type InteractionClass { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool UseLocalizedNaming { get; set; } = false;
    public bool Nsfw { get; set; } = false;
    public GuildPermission? GuildPermission { get; set; }
    public HashSet<InteractionContextType>? ContextTypes { get; set; }
    public HashSet<ApplicationIntegrationType>? IntegrationTypes { get; set; }

    public List<OptionsData> Options { get; set; } = new();
}