namespace Papageis.DiscordNet.Module;

public interface IBaseBotModule
{
    /// <summary>
    /// This is an async implementation to Load EventHandlers
    /// </summary>
    public Task InitializeAsync();

    /// <summary>
    /// This is an async implementation to Unload EventHandlers
    /// </summary>
    public Task UnregisterAsync();
}