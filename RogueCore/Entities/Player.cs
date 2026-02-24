using RogueCore.Helpers;
using RogueCore.Models;

namespace RogueCore.Entities;

public class Player : Character
{
    public int numOfCoins;
    public Player(int x, int y) : base(x, y)
    {
        _maxHealth = GameSettings.Player.MaxHealth;
        _health = _maxHealth;
    }

    protected override void CheckTile(Tile tile)
    {
        switch (tile.type)
        {
            case TileType.Coin:
                AddCoins(1, tile);
                break;
            case TileType.HealthPotion:
                AddHealth(10);
                tile.type = TileType.Floor;
                break;
            default:
                break;
        }
    }
    
    public void AddCoins(int amount, Tile tile)
    {
        tile.type = TileType.Floor;
        numOfCoins += amount;
    }

    public override void Die()
    {
        throw new NotImplementedException();
    }
}