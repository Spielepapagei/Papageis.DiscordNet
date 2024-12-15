using Papageis.DiscordNet.Configuration;

namespace Papapageis.DiscordNet.Test.Configuration;

public class AppConfiguration
{
    public HoneypotData Honeypot { get; set; } = new();
    public DiscordBotConfiguration DiscordBotConfiguration { get; set; } = new();
    public DiscordBotConfigurationExtData DiscordBotConfigurationExt { get; set; } = new();
    
    
    
    public class HoneypotData
    {
        public List<SupportedGuildsData> SupportedGuilds { get; set; } = new()
        {
            new SupportedGuildsData
            {
                UrlWithHttpsAndSlash = "",
                GuildId = 0
            }
        };
        
        public class SupportedGuildsData
        {
            public string UrlWithHttpsAndSlash { get; set; } = string.Empty;
            public ulong GuildId { get; set; }
        }
    }

    public class DiscordBotConfigurationExtData
    {
        public string Token { get; set; } = string.Empty;
        
        public ulong AdminLogGuildId { get; set; }
        
        public ulong AdminLogChannelId { get; set; }

        public int MaxInvitesPerMinute { get; set; } = 20;
    }
}