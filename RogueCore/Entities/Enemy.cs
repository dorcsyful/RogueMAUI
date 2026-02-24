using RogueCore.Models;

namespace RogueCore.Entities;

public class Enemy : Character
{
    private Room _room;
    private float attackCooldown = 0f;

    public Enemy(int x, int y, Room room) : base(x, y, false)
    {
        _room = room;
    }
    
    public override void Die()
    {
        throw new NotImplementedException();
    }

    protected override void CheckTile(Tile tile)
    {
        throw new NotImplementedException();
    }
}