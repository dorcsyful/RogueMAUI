using RogueCore.Helpers;
using RogueCore.Models;

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
    
    private Random _random;
    private BSP _bsp;
    private MST _mst;
    private List<Cell> _cells;
    private List<Cell> _corridors;
    private List<Tile> _tiles;
    private List<List<Tile>> _map;
    public DungeonGenerator(Random random)
    {
        _random = random;
        
        _bsp = new BSP(_random);
        _mst = new MST(_random);
        
        _cells = new List<Cell>();
        _corridors = new List<Cell>();
        _tiles = new List<Tile>();
    }
    public List<Cell> GetCells() { return _cells; }
    public List<Cell> GetCorridors() { return _corridors; }
    public void GenerateDungeon()
    {

        Cell root = _bsp.Generate(GameSettings.Dungeon.MapWidth,GameSettings.Dungeon.MapHeight);
        SortToList(root);
        Trim();
        BuildMap();
        SnapshotService.SaveMapSnapshot(SnapshotService.ConvertMapToTileTypes(_map), "1_BSP"); 
        
        _corridors = _mst.CreateCorridors(_cells);
        BuildMap();
        SnapshotService.SaveMapSnapshot(SnapshotService.ConvertMapToTileTypes(_map), "2_MST");
        
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

    
    public void BuildMap()
    {
        _map = new List<List<Tile>>();

        for (int x = 0; x < GameSettings.Dungeon.MapWidth; x++)
        {
            List<Tile> column = new List<Tile>(new Tile[GameSettings.Dungeon.MapHeight]); // Creates a row of 0s
            for (int j = 0; j < column.Count; j++)
            {
                column[j] = new Tile(x, j, Models.TileType.Empty);
            }
            _map.Add(column);
        }
        for (int i = 0; i < _cells.Count; i++)
        {
            for (int j = _cells[i].x1; j < _cells[i].x2; j++)
            {
                for (int k = _cells[i].y1; k < _cells[i].y2; k++)
                {
                    _map[j][k] = new Tile(j,k,Models.TileType.Floor);
                }
            }
        }
        
        for(int i = 0; i < _corridors.Count; i++)
        {
            for (int j = _corridors[i].x1; j < _corridors[i].x2; j++)
            {
                for (int k = _corridors[i].y1; k < _corridors[i].y2; k++)
                {
                    _map[j][k] = new Tile(j,k,Models.TileType.Corridor);
                }
            }
        }

        foreach (Tile tile in _tiles)
        {
            _map[tile.x][tile.y] = new Tile(tile.x, tile.y, tile.type);
        }
    }

    public List<List<Tile>> GetMap() { return _map; }
}