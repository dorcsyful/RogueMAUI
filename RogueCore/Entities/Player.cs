using RogueCore.Helpers;
using RogueCore.Models;

namespace RogueCore.Entities;

public class Player : Character
{
    public int numOfCoins;
    public bool reachedExit = false;
    public Player(int x, int y) : base(x, y, true)
    {
        _maxHealth = GameSettings.Player.MaxHealth;
        _health = _maxHealth;
        IsPlayer = true;
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
            case TileType.Exit:
                reachedExit = true;
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
        Console.WriteLine("Player has died!");
        _isDead = true;
    }
}