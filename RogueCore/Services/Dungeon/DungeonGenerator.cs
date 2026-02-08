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
        public List<Cell> vertical_Neighbours;
        public List<Cell> horizontal_Neighbours;
        public bool isCorridor;
        public Cell(int x1, int x2, int y1, int y2)
        {
            isCorridor = false;
            vertical_Neighbours = new List<Cell>();
            horizontal_Neighbours = new List<Cell>();
            left = null; right = null;
            this.x1 = x1;
            this.x2 = x2;
            this.y1 = y1;
            this.y2 = y2;
        }
    }

    private Random _random = new Random();
    private BSP _bsp;
    private List<Cell> _cells;
    private List<Cell> _corridors;
    public DungeonGenerator()
    {
        _bsp = new BSP(_random);
        _cells = new List<Cell>();
    }
    public void GenerateDungeon()
    {
        _cells = new List<Cell>();
        _corridors = new List<Cell>();
        Cell root = _bsp.Generate(GameSettings.Dungeon.MapWidth,GameSettings.Dungeon.MapHeight);
        SortToList(root);
        FindNeighbours();
        Trim();
        _cells.AddRange(_corridors);
        
        SnapshotService.SaveMapSnapshot(SnapshotService.ConvertCellToGrid(_cells), "BSP1");
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
    private void FindNeighbours()
    {
        foreach (Cell cell in _cells)
        {
            foreach (Cell other in _cells)
            {
                if(cell == other) continue;
                if (cell.x2 == other.x1)
                {
                    if (Math.Max(cell.y1, other.y1) < Math.Min(cell.y2, other.y2))
                    {
                        cell.horizontal_Neighbours.Add(other);
                    }
                }

                if (cell.y1 == other.y2)
                {
                    if (Math.Max(cell.x1, other.x1) < Math.Min(cell.x2, other.x2))
                    {
                        cell.vertical_Neighbours.Add(other);
                    }
                }
            }
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