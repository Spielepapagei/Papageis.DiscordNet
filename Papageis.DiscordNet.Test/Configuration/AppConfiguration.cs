namespace Papapageis.DiscordNet.Test.Configuration;

public class AppConfiguration
{
    public SettingsData Settings { get; set; } = new();
    public string BotToken { get; set; } = string.Empty;
    
    public class SettingsData
    {
        public bool Enable { get; set; } = true;
        public bool EnableDebug { get; set; } = false;
    }
}