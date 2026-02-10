using System.Security.Cryptography;
using RogueCore.Models;

namespace RogueCore.Services.Dungeon;

public class MapGenerator
{
    private DungeonGenerator _dungeonGenerator;
    private InteriorDesign _interiorDesign;
    private Random _random;
    private List<List<Tile>> _map;
    private List<Room> _rooms;
    private List<Corridor> _corridors;
    
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
        CellToCorridor();
        _interiorDesign.AssignMapRoom(_map,_rooms);
        _interiorDesign.CreateEntranceExit();
        SnapshotService.SaveMapSnapshot(SnapshotService.ConvertMapToTileTypes(_map), "3_EntranceExit");
        _interiorDesign.PlaceCoins();
        SnapshotService.SaveMapSnapshot(SnapshotService.ConvertMapToTileTypes(_map), "4_Coins");
        
    }
    
    private void CellToRoom()
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

    private Room? FindRoomByCoordinates(int x1, int x2, int y1, int y2)
    {
        foreach (Room room in _rooms)
        {
            if(room.x1 == x1 && room.x2 == x2 && room.y1 == y1 && room.y2 == y2) return room;
        }
        return null;
    }
    
    private void CellToCorridor()
    {
        _corridors = new List<Corridor>();
        foreach (DungeonGenerator.Cell cell in _dungeonGenerator.GetCorridors())
        {
            Room start = FindRoomByCoordinates(cell.left.x1, cell.left.x2, cell.left.y1, cell.left.y2);
            Room end = FindRoomByCoordinates(cell.right.x1, cell.right.x2, cell.right.y1, cell.right.y2);
            if (start == null || end == null) { throw new Exception("Could not find room"); }
            Corridor corridor = new Corridor(start,end,cell.x1, cell.x2, cell.y1, cell.y2);
            for(int i = cell.x1; i <= cell.x2; i++)
            {
                for(int j = cell.y1; j <= cell.y2; j++)
                {
                    Tile tile = new Tile(i, j, Models.TileType.Corridor);
                    tile.room = null;
                    corridor.AddTile(tile);
                    
                    _map[i][j] = tile;
                }
            }
            _corridors.Add(corridor);
        }
    }
    
    public List<List<Tile>> GetMap() => _map;
    public List<Room> GetRooms() => _rooms;
    public List<Corridor> GetCorridors() => _corridors;
}