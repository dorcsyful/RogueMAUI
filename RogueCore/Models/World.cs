using RogueCore.Entities;
using RogueCore.Services.Dungeon;

namespace RogueCore.Models;

public class World
{
    public List<List<Tile>> Map;
    public List<Room> Rooms;
    public List<Corridor> Corridors;
    public Player Player;
    
    private MapGenerator _mapGenerator;
    
    public World()
    {
        _mapGenerator = new MapGenerator();
        _mapGenerator.GenerateMap();
        Map = _mapGenerator.GetMap();
        Rooms = _mapGenerator.GetRooms();
        Corridors = _mapGenerator.GetCorridors();
        Player = new Player();
    }
    
    
    
}