namespace RogueCore.Models;

public class Corridor
{
    public Room start;
    public Room end;
    public int x1,x2,y1,y2;
    private List<List<Tile?>> _map;

    public Corridor(Room start, Room end, int x1, int x2, int y1, int y2)
    {
        this.start = start;
        this.end = end;
        this.x1 = x1;
        this.x2 = x2;
        this.y1 = y1;
        this.y2 = y2;
        _map = new List<List<Tile?>>();
        for(int i = 0; i < x2-x1+1; i++)
        {
            List<Tile?> row = new List<Tile?>();
            for(int j = 0; j < y2-y1+1; j++)
            {
                row.Add(null);
            }
            _map.Add(row);
        }
    }
    public bool AddTile(Tile tile)
    {
        if (tile.x >= x1 && tile.x <= x2 && tile.y >= y1 && tile.y <= y2)
        {
            if (_map[tile.x - x1][tile.y - y1] == null)
            {
                _map[tile.x - x1][tile.y -y1] = tile;
            }

            return true;
        }
        return false;
    }
}