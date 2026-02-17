using RogueCore.Models;

namespace RogueMAUI.Graphics;

public static class TileCoordinates
{
    // Each sprite is 16x16 pixels in the tilemap
    public const int SpriteSize = 16;
    
    // Tile coordinates (x, y) in pixels on tilemap.png
    public static readonly Dictionary<TileType, (int x, int y)> Tiles = new()
    {
        { TileType.Empty, (0, 0) },
        { TileType.Floor, (16, 0) },
        { TileType.Corridor, (32, 0) }, // Use floor tile for corridor
        { TileType.Entrance, (48, 0) },
        { TileType.Exit, (64, 0) },
        { TileType.Coin, (48, 16) },
        {TileType.HealthPotion, (64, 0) },
        { TileType.PoisonPotion , (64, 32) },
    };
    
    public static int[] GetTileBaseCoordinates(TileType type)
    {
        if(type == TileType.Empty) return new int[] {Tiles[TileType.Empty].x, Tiles[TileType.Empty].y};
        if(type ==TileType.Corridor) return new int[] {Tiles[TileType.Corridor].x, Tiles[TileType.Corridor].y};
        else return new int[] {Tiles[TileType.Floor].x, Tiles[TileType.Floor].y};
    }
    
    public static int[] GetDecorationTileCoordinates(TileType type)
    {
        if(type == TileType.Entrance) return new int[] {Tiles[TileType.Entrance].x, Tiles[TileType.Entrance].y};
        if(type == TileType.Exit) return new int[] {Tiles[TileType.Exit].x, Tiles[TileType.Exit].y};
        if(type == TileType.Coin) return new int[] {Tiles[TileType.Coin].x, Tiles[TileType.Coin].y};
        if(type == TileType.HealthPotion) return new int[] {Tiles[TileType.HealthPotion].x, Tiles[TileType.HealthPotion].y};
        if(type == TileType.PoisonPotion) return new int[] {Tiles[TileType.PoisonPotion].x, Tiles[TileType.PoisonPotion].y};
        else return new int[] {Tiles[TileType.Floor].x, Tiles[TileType.Floor].y};
    }
    
    // Player sprite coordinates
    public static class Player
    {
        public static readonly (int x, int y) Idle = (0, 48);
        
        // 6-frame run animation
        public static readonly (int x, int y)[] RunAnimation = 
        {
            (0, 64),
            (16, 64),
            (32, 64),
            (48, 64),
            (64, 64),
            (80, 64)
        };
    }
    
    // Enemy sprite coordinates
    public static class Enemy
    {
        public static readonly (int x, int y) Idle = (16, 48);
        
        // 6-frame run animation
        public static readonly (int x, int y)[] RunAnimation = 
        {
            (0, 80),
            (16, 80),
            (32, 80),
            (48, 80),
            (64, 80),
            (80, 80)
        };
    }
}
