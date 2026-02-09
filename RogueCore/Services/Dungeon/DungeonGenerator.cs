using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using RogueCore.Helpers;

namespace RogueCore.Services.Dungeon;

public class DungeonGenerator
{
    public class Cell
    {
        public int x1,x2,y1,y2;
        public Cell? left,right;
        public Cell(int x1, int x2, int y1, int y2)
        {
            left = null; right = null;
            this.x1 = x1;
            this.x2 = x2;
            this.y1 = y1;
            this.y2 = y2;
        }
    }

    private Random _random = new Random();
    private BSP _bsp;
    private MST _mst;
    private List<Cell> _cells;
    private List<Cell> _corridors;
    public DungeonGenerator()
    {
        _bsp = new BSP(_random);
        _mst = new MST(_random);
        
        _cells = new List<Cell>();
        _corridors = new List<Cell>();
    }
    public void GenerateDungeon()
    {

        Cell root = _bsp.Generate(GameSettings.Dungeon.MapWidth,GameSettings.Dungeon.MapHeight);
        SortToList(root);
        Trim();
        _corridors = _mst.CreateCorridors(_cells);

        SnapshotService.SaveMapSnapshot(SnapshotService.ConvertCellToGrid(_cells,_corridors), "BSP");
        
        _cells.AddRange(_corridors);
        
        SnapshotService.SaveMapSnapshot(SnapshotService.ConvertCellToGrid(_cells,_corridors), "MST");
    }
    private void SortToList(DungeonGenerator.Cell root)
    {
        if (root.left == null || root.right == null)
        {
            _cells.Add(root);
        }
        else
        {
            SortToList(root.left);
            SortToList(root.right);
        }
    }

    private void Trim()
    {
        for (int i = 0; i < _cells.Count; i++)
        {

            float horizontalShrink = _random.Next(20, 40) / 100f;
            float verticalShrink = _random.Next(20, 40) / 100f;
            int newHorizontalSize = (int)(Math.Floor((_cells[i].x2 - _cells[i].x1) * horizontalShrink) / 2);
            int newVerticalSize = (int)(Math.Floor((_cells[i].y2 - _cells[i].y1) * verticalShrink) / 2);
            _cells[i].x1 +=  newHorizontalSize;
            _cells[i].x2 -=  newHorizontalSize;
            
            _cells[i].y1 +=  newVerticalSize;
            _cells[i].y2 -=  newVerticalSize;
        }
    }
}