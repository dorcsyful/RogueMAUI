using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using RogueCore.Helpers;
using RogueCore.Models;
using RogueCore.Services.Dungeon;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Color = SixLabors.ImageSharp.Color;

namespace RogueCore.Services;
public static class SnapshotService
{
    //Fully AI generated. I cannot be bothered
    public static void SaveMapSnapshot(List<List<TileType>> map, string fileName)
    {
        int width = map.Count;
        int height = map[0].Count;
        int scale = 5; // Make each tile 10x10 pixels

        // 1. Create the image in memory
        using Image<Rgba32> image = new Image<Rgba32>(width * scale, height * scale);

        // 2. Fill it with logic (The Backend way)
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Determine color based on tile type
                Color tileColor = map[x][y] switch
                {
                    TileType.Empty => Color.DarkGray,
                    TileType.Floor => Color.White,
                    TileType.Corridor => Color.Red,
                    TileType.Entrance => Color.Green,
                    TileType.Exit => Color.Blue,
                    _ => Color.Purple
                };
                // Draw the "tile" by coloring pixels
                for (int i = 0; i < scale; i++)
                {
                    for (int j = 0; j < scale; j++)
                    {
                        image[x * scale + i, y * scale + j] = tileColor;
                    }
                }
            }
        }

        // 3. Save to the app's cache directory
        string path = "Snapshots/ " + fileName+".bmp";
        image.SaveAsPng(path);
        
        // This line is great for debugging in Rider's terminal
        Console.WriteLine($"SNAPSHOT CREATED: {path}");
    }

    public static List<List<TileType>> ConvertCellToGrid(List<DungeonGenerator.Cell> floors, List<DungeonGenerator.Cell> corridors, List<DungeonGenerator.Tile> tiles)
    {
        List<List<TileType>> map = new List<List<TileType>>();
        for (int x = 0; x < GameSettings.Dungeon.MapWidth; x++)
        {
            List<TileType> column = new List<TileType>(new TileType[GameSettings.Dungeon.MapHeight]); // Creates a row of 0s
            map.Add(column);
        }

        InsertType(floors, map, TileType.Floor);
        InsertType(corridors, map, TileType.Corridor);
        
        foreach (var t in tiles)
        {
            map[t.x][t.y] = t.type;
        }

        
        return map;
    }

    private static void InsertType(List<DungeonGenerator.Cell> floors, List<List<TileType>> map, TileType type)
    {
        for (int i = 0; i < floors.Count; i++)
        {
            for (int j = floors[i].x1; j < floors[i].x2; j++)
            {
                for (int k = floors[i].y1; k < floors[i].y2; k++)
                {
                    if(type == TileType.Corridor && map[j][k] == TileType.Floor) continue; // Don't overwrite rooms with corridors
                    map[j][k] = type;
                }
            }
        }
    }
}