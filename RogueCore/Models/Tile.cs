using RogueCore.Services.Dungeon;

namespace RogueCore.Models;

public enum TileType
{
    Empty = 0,
    Floor = 1,
    Corridor = 2,
    Entrance = 3,
    Exit = 4,
    Coin = 5,
    HealthPotion = 6,
    PoisonPotion = 7,
    Space = 8
}

public class Tile
{
    public int x,y;
    public Models.TileType type;
    public Room? room; // The room this tile belongs to, if any
    public Tile(int x, int y, Models.TileType type)
    {
        this.x = x;
        this.y = y;
        this.type = type;
    }
}