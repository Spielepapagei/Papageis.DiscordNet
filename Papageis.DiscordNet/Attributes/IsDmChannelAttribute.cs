using Discord;

namespace Papageis.DiscordNet.Attributes;

public class IsDmChannelAttribute : Attribute
{
    public IsDmChannelAttribute(bool canBeDm)
    {
        CanBeDm = canBeDm;
    }
    public bool CanBeDm { get; set; }
}