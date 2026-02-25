using System.Diagnostics;
using RogueCore.Models;

namespace RogueCore.Entities;

public class Enemy : Character
{
    private Room _room;
    private float attackCooldown = 1f;

    public Enemy(int x, int y, Room room) : base(x, y, false)
    {
        _health = 100;
        _room = room;
    }
    
    public override void Die()
    {
            _isDead = true;
            // Additional logic for enemy death (e.g., drop loot, play animation)
    }

    protected override void CheckTile(Tile tile)
    {
        throw new NotImplementedException();
    }
}