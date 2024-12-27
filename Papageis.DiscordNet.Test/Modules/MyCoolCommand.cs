using Discord;
using Discord.WebSocket;
using Papageis.DiscordNet.Attributes;
using Papageis.DiscordNet.Module;

namespace Papapageis.DiscordNet.Test.Modules;

public class MyCoolCommand : IBaseSlashCommand
{
    public string Name { get; set; }

    public async Task<SlashCommandBuilder> RegisterAsync()
    {
        return new SlashCommandBuilder()
            .WithName("testy")
            .WithDescription("This is a TestCommand")
            .AddOption("ping", ApplicationCommandOptionType.String, "type something here", false);
    }

    [IsBot(true)]
    [IsDmChannel(true)]
    [HasPermission(GuildPermission.Administrator)]
    public async Task CommandExecuted(SocketSlashCommand command)
    {
        await command.RespondAsync("pong");
    }
}