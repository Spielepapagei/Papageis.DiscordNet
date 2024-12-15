using Discord;

namespace Papageis.DiscordNet.Attributes;

public class HasPermissionAttribute : Attribute
{
    public HasPermissionAttribute(GuildPermission permission)
    {
        Permission = permission;
    }
    public GuildPermission Permission { get; set; }
}