using RogueCore.Entities;

namespace RogueCore.Models;

public enum EventType
{
    SlashAttack,
    ExplosionDeath
}

public class Event
{
     public EventType Type { get; set; }
     public DateTime StartTime { get; set; } = DateTime.Now;
     public float LifespanSeconds { get; set; } = 1.0f;     public float X { get; set; }
     public float Y { get; set; }
     
     public void RemoveEffect(Character? character)
     {
         character?.DisableDamage();
     }
}