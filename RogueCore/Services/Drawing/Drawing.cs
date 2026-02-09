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

    public static List<List<TileType>> ConvertMapToTileTypes(List<List<DungeonGenerator.Tile>> map)
    {
        List<List<TileType>> tileTypeMap = new List<List<TileType>>();
        for (int x = 0; x < map.Count; x++)
        {
            List<TileType> column = new List<TileType>();
            for (int y = 0; y < map[x].Count; y++)
            {
                column.Add(map[x][y].type);
            }
            tileTypeMap.Add(column);
        }
        return tileTypeMap;
    }

}