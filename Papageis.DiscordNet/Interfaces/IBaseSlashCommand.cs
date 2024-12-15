using Discord;

namespace Papageis.DiscordNet.Module;

public interface IBaseSlashCommand
{
    /// <summary>
    /// This is an async implementation to Registering SlashCommands
    /// </summary>
    public Task<SlashCommandBuilder> RegisterAsync();
}