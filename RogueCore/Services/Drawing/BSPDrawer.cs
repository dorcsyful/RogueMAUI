using System.Collections.Generic;
using RogueCore.Services.Dungeon;

namespace RogueCore.Services;

//written by AI because it's too much effort to write it myself
public static class BSPDrawer
{
    public static List<List<int>> ConvertBspToGrid(DungeonGenerator.Cell root, int totalWidth, int totalHeight)
    {
        // 1. Initialize the grid with 0s (Walls)
        var grid = new List<List<int>>();
        for (int y = 0; y < totalHeight; y++)
        {
            var row = new List<int>(new int[totalWidth]); // Creates a row of 0s
            grid.Add(row);
        }

        // 2. Start the recursive fill process
        FillGridFromBsp(root, grid);

        return grid;
    }

    private static void FillGridFromBsp(DungeonGenerator.Cell node, List<List<int>> grid)
    {
        if (node == null) return;

        // Check if it's a leaf node (null children)
        if (node.left == null && node.right == null)
        {
            // 3. Fill the area encompassed by this leaf node
            // We use <= x2/y2 if your coordinates are inclusive bounds
            for (int y = node.y1; y < node.y2; y++)
            {
                for (int x = node.x1; x < node.x2; x++)
                {
                    // Safety check to prevent IndexOutOfRangeException
                    if (y >= 0 && y < grid.Count && x >= 0 && x < grid[0].Count)
                    {
                        grid[y][x] = 1; // Mark as Floor
                    }
                }
            }
        }
        else
        {
            // If not a leaf, keep digging down the tree
            FillGridFromBsp(node.left, grid);
            FillGridFromBsp(node.right, grid);
        }
    }
}