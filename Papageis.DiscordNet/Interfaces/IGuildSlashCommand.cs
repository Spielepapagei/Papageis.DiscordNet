using Discord.Interactions.Builders;

namespace Papageis.DiscordNet.Module;

public interface IGuildSlashCommand : IBaseSlashCommand
{
    /// <summary>
    /// Set the Guild where the SlashCommand should be Registered
    /// </summary>
    public Task<ulong> GuildId();
}