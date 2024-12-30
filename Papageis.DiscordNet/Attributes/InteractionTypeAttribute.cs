namespace Papageis.DiscordNet.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class InteractionTypeAttribute : Attribute
{
    private readonly string Name;
    private InteractionType Type;

    public InteractionTypeAttribute(string name, InteractionType interactionType)
    {
        Name = name;
        Type = interactionType;
    }

    public string GetName() => Name;
    public InteractionType GetInteractionType() => Type;
}