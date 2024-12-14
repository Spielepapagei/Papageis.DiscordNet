namespace Papageis.DiscordNet.Module;

public interface IGuildSlashCommand : IBaseSlashCommand
{
    /// <summary>
    /// Set the Guild where the Command should be Registered
    /// </summary>
    public Task<ulong> GuildId();
}