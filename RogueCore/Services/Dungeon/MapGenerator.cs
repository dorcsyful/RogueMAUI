using RogueCore.Models;

namespace RogueCore.Services.Dungeon;

public class MapGenerator
{
    private DungeonGenerator _dungeonGenerator;
    private InteriorDesign _interiorDesign;
    private Random _random;
    private List<List<Tile>> _map;
    private List<Room> _rooms;
    
    public MapGenerator()
    {
        _random = new Random();
        _dungeonGenerator = new DungeonGenerator(_random);
        _interiorDesign = new InteriorDesign(_random);
    }
    
    public void GenerateMap()
    {
        _dungeonGenerator.GenerateDungeon();
        _map = _dungeonGenerator.GetMap();
        CellToRoom();
        _interiorDesign.AssignMapRoom(_map,_rooms);
        _interiorDesign.CreateEntranceExit();
        SnapshotService.SaveMapSnapshot(SnapshotService.ConvertMapToTileTypes(_map), "3_EntranceExit");

        
    }
    
    public void CellToRoom()
    {
        _rooms = new List<Room>();
        foreach (DungeonGenerator.Cell cell in _dungeonGenerator.GetCells())
        {
            Room room = new Room(cell.x1, cell.x2, cell.y1, cell.y2);
            for(int i = cell.x1; i <= cell.x2; i++)
            {
                for(int j = cell.y1; j <= cell.y2; j++)
                {
                    Tile tile = new Tile(i, j, Models.TileType.Floor);
                    tile.room = room;
                    room.AddTile(tile);
                    _map[i][j] = tile;
                }
            }
            _rooms.Add(room);
        }
    }
}