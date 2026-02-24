namespace RogueCore.Models;

public enum EventType
{
    SlashAttack,
    ExplosionDeath
}

public class Event
{
     public EventType Type { get; set; }
     public DateTime Duration { get; set; }
     public float X { get; set; }
     public float Y { get; set; }
}