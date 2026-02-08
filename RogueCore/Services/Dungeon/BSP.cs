using System;
using System.Collections.Generic;
using System.Diagnostics;
using RogueCore.Helpers;
using RogueCore.Services.Dungeon;
namespace RogueCore.Services.Dungeon;

public class BSP
{
    private Random _random;
    public BSP(Random random)
    {
        _random = random;
    }
    public DungeonGenerator.Cell Generate(int width, int height)
    {
        Debug.WriteLine("Start");
        Queue<DungeonGenerator.Cell> splitQueue = new Queue<DungeonGenerator.Cell>();
        //magic number for a bit of padding
        DungeonGenerator.Cell root = new DungeonGenerator.Cell(5,width - 5, 5, height - 5);
        splitQueue.Enqueue(root);
        int cellCount = 1;
        bool split = true;
        while (splitQueue.Count > 0)
        {
            DungeonGenerator.Cell cell = splitQueue.Dequeue();
            if (cell.left == null && cell.right == null)
            {
                if (Split(cell))
                {
                    cellCount++;
                    split = true;
                    splitQueue.Enqueue(cell.left);
                    splitQueue.Enqueue(cell.right);
                }
            }
        }

        return root;
        List<List<int>> flattened =BSPDrawer.ConvertBspToGrid(root,GameSettings.Dungeon.MapWidth, GameSettings.Dungeon.MapHeight);
        // SnapshotService.SaveMapSnapshot(flattened, "BSP1");
    }
    private bool Split(DungeonGenerator.Cell cell)
    {
        int width = cell.x2 - cell.x1;
        int height = cell.y2 - cell.y1;
        if (width < GameSettings.Dungeon.MinRoomWidth * 2 && height < GameSettings.Dungeon.MinRoomHeight * 2)
        {
            return false;
        }

        bool isWidthLonger = width >= height;
        bool canHalveWidth = width >= GameSettings.Dungeon.MaxRoomWidth + 1;
        bool canHalveHeight = height >= GameSettings.Dungeon.MaxRoomWidth + 1;
        
        if (!canHalveWidth && !canHalveHeight) return false;
        if (isWidthLonger)
        {
            if(canHalveWidth)
            {
                HalveWidth();
            }

            else if (canHalveHeight)
            {
                HalveHeight();
            }
        }
        else
        {
            if(canHalveHeight) HalveHeight();
            else if(canHalveWidth) HalveWidth();
        }
        return true;

        void HalveWidth()
        {
            int splitAt = _random.Next(GameSettings.Dungeon.MinRoomWidth,
                width - GameSettings.Dungeon.MinRoomWidth);
            cell.left = new DungeonGenerator.Cell(cell.x1, cell.x1 + splitAt, cell.y1, cell.y2);
            cell.right = new DungeonGenerator.Cell(cell.x1 + splitAt, cell.x2, cell.y1, cell.y2);
        }

        void HalveHeight()
        {
            int splitAt = _random.Next(GameSettings.Dungeon.MinRoomHeight, height - GameSettings.Dungeon.MinRoomHeight);
            cell.left = new DungeonGenerator.Cell(cell.x1,cell.x2, cell.y1, cell.y1 + splitAt);
            cell.right = new DungeonGenerator.Cell(cell.x1, cell.x2, cell.y1 + splitAt, cell.y2);
        }
    }
    
}