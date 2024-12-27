using Discord;
using Discord.WebSocket;

namespace Papageis.DiscordNet.Module;

public interface IBaseUserCommand
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
    /// This is an async implementation to Registering UserCommands
    /// </summary>
    public Task<UserCommandBuilder> RegisterAsync();
    
    /// <summary>
    /// Here you Implement the logic for this UserCommand
    /// </summary>
    public Task CommandExecuted(SocketUserCommand command);
}