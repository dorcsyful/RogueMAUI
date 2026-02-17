using RogueCore.Entities;
using RogueCore.Helpers;
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
        Player.SetPosition(_mapGenerator.GetInteriorDesign().GetEntrance().x, _mapGenerator.GetInteriorDesign().GetEntrance().y);
    }
    
    public bool TryMovePlayer(int dx, int dy)
    {
        int newX = Player.GetX() + dx;
        int newY = Player.GetY() + dy;

        if (IsWalkable(newX, newY))
        {
            Player.Move(dx, dy);
            return true;
        }
        return false;
    }

    private bool IsWalkable(int newX, int newY)
    {
        if(Map[newX][newY].type != TileType.Empty) return true;
        return false;
    }
}