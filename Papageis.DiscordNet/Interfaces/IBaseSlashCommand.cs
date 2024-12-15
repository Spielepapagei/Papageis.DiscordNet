using Discord;
using Discord.WebSocket;

namespace Papageis.DiscordNet.Module;

public interface IBaseSlashCommand
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
    /// This is an async implementation to Registering SlashCommands
    /// </summary>
    public Task<SlashCommandBuilder> RegisterAsync();
    
    /// <summary>
    /// Here you Implement the logic for this Command
    /// </summary>
    public Task CommandExecuted(SocketSlashCommand command);
}