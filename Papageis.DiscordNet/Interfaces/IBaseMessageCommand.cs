using Discord;
using Discord.WebSocket;

namespace Papageis.DiscordNet.Module;

public interface IBaseMessageCommand
{
    public async Task GetName()
    {
        Name = RegisterAsync().Result.Name;
    }
    
    /// <summary>
    /// Gives you back the name of the Command
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// This is an async implementation to Registering MessageCommands
    /// </summary>
    public Task<MessageCommandBuilder> RegisterAsync();
    
    /// <summary>
    /// Here you Implement the logic for this MessageCommand
    /// </summary>
    public Task CommandExecuted(SocketMessageCommand command);
}