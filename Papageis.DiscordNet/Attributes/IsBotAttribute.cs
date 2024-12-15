namespace Papageis.DiscordNet.Attributes;

public class IsBotAttribute : Attribute
{
    public IsBotAttribute(bool canBeBot)
    {
        CanBeBot = canBeBot;
    }
    public bool CanBeBot { get; set; }
}