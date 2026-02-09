using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using RogueCore.Helpers;
using RogueCore.Services.Dungeon;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Color = SixLabors.ImageSharp.Color;

namespace RogueCore.Services;
public static class SnapshotService
{
    public static void SaveMapSnapshot(List<List<int>> map, string fileName)
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
                    1 => Color.White,     // Floor
                    2 => Color.LightBlue,  // Room
                    _ => Color.Black       // Wall
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
        string path = fileName+".bmp";
        image.SaveAsPng(path);
        
        // This line is great for debugging in Rider's terminal
        Console.WriteLine($"SNAPSHOT CREATED: {path}");
    }

    public static List<List<int>> ConvertCellToGrid(List<Dungeon.DungeonGenerator.Cell> cells)
    {
        List<List<int>> map = new List<List<int>>();
        for (int x = 0; x < GameSettings.Dungeon.MapWidth; x++)
        {
            List<int> column = new List<int>(new int[GameSettings.Dungeon.MapHeight]); // Creates a row of 0s
            map.Add(column);
        }

        for (int i = 0; i < cells.Count; i++)
        {
            for (int j = cells[i].x1; j < cells[i].x2; j++)
            {
                for (int k = cells[i].y1; k < cells[i].y2; k++)
                {
                    map[j][k] = 1;
                }
            }
        }
        return map;
    }
}