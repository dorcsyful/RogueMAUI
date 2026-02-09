using System;
using System.Collections.Generic;

namespace RogueCore.Services.Dungeon;

public class MST
{
    private Random _random;
    private List<DungeonGenerator.Cell>? _corridors;

    public MST(Random random)
    {
        _random = random;
    }

    public List<DungeonGenerator.Cell> CreateCorridors(List<DungeonGenerator.Cell> cells)
    {
        _corridors = new List<DungeonGenerator.Cell>();
        List<DungeonGenerator.Cell> reached = new List<DungeonGenerator.Cell>();
        List<DungeonGenerator.Cell> unreached = new List<DungeonGenerator.Cell>(cells);

        reached.Add(unreached[0]);
        unreached.RemoveAt(0);

        while (unreached.Count > 0)
        {
            float minDist = float.MaxValue;
            DungeonGenerator.Cell? bestA = null;
            DungeonGenerator.Cell? bestB = null;

            foreach (DungeonGenerator.Cell r in reached)
            {
                foreach (DungeonGenerator.Cell u in unreached)
                {
                    float d = GetDistance(r, u);
                    if (d < minDist)
                    {
                        minDist = d;
                        bestA = r;
                        bestB = u;
                    }
                }
            }

            if (bestA != null && bestB != null)
            {
                int x1 = (bestA.x1 + bestA.x2) / 2;
                int y1 = (bestA.y1 + bestA.y2) / 2;
                int x2 = (bestB.x1 + bestB.x2) / 2;
                int y2 = (bestB.y1 + bestB.y2) / 2;

                
                CreateLShapedCorridor(x1, y1, x2, y2, bestA, bestB);

                reached.Add(bestB);
                unreached.Remove(bestB);
            }
        }

        return _corridors;
    }
    
    public static float GetDistance(DungeonGenerator.Cell a, DungeonGenerator.Cell b)
    {
        int ax = (a.x1 + a.x2) / 2;
        int ay = (a.y1 + a.y2) / 2;
        int bx = (b.x1 + b.x2) / 2;
        int by = (b.y1 + b.y2) / 2;

        return (float)Math.Sqrt(Math.Pow(ax - bx, 2) + Math.Pow(ay - by, 2));
    }
    
    private void CreateLShapedCorridor(int x1, int y1, int x2, int y2, DungeonGenerator.Cell cellA, DungeonGenerator.Cell cellB)
    {
        if (Math.Abs(x1 - x2) < 3) x1 = x2;
        if (Math.Abs(y1 - y2) < 3) y1 = y2;
        //
        if (x1 == x2)
        {
            if (y1 < y2)
            {
                _corridors.Add(new DungeonGenerator.Cell(x1, x1 + 1,cellA.y2, cellB.y1));
            }
            else
            {
                _corridors.Add(new DungeonGenerator.Cell(x1, x1 + 1,cellA.y1, cellB.y2));
            }
            
        }
        else
        {
            if (x1 < x2)
            {
                _corridors.Add(new DungeonGenerator.Cell(cellA.x2, cellB.x1,y1, y1 + 1));
            }
            else
            {
                _corridors.Add(new DungeonGenerator.Cell(cellB.x1, cellA.x2,y1, y1 + 1));
            }
        }

        return;
        
        {
            if (_random.Next(2) == 0)
            {
                _corridors.Add(new DungeonGenerator.Cell(Math.Min(x1, x2), Math.Max(x1, x2) + 1, y1, y1 + 1));
                _corridors.Add(new DungeonGenerator.Cell(x2, x2 + 1, Math.Min(y1, y2), Math.Max(y1, y2) + 1));
            }
            else
            {
                _corridors.Add(new DungeonGenerator.Cell(x1, x1 + 1, Math.Min(y1, y2), Math.Max(y1, y2) + 1));
                _corridors.Add(new DungeonGenerator.Cell(Math.Min(x1, x2), Math.Max(x1, x2) + 1, y2, y2 + 1));
            }
        }
        

    }


}