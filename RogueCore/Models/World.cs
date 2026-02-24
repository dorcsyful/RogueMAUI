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
    public List<Enemy> Enemies;
    private MapGenerator _mapGenerator;
    public List<Event> Events { get; private set; }

    public World()
    {
        Events = new List<Event>();

        _mapGenerator = new MapGenerator();
        _mapGenerator.GenerateMap();
        Map = _mapGenerator.GetMap();
        Rooms = _mapGenerator.GetRooms();
        Corridors = _mapGenerator.GetCorridors();
        Player = new Player(_mapGenerator.GetEntranceCoordinates().x, _mapGenerator.GetEntranceCoordinates().y);
        Player.SetPosition(_mapGenerator.GetInteriorDesign().GetEntrance().x, _mapGenerator.GetInteriorDesign().GetEntrance().y);
        Enemies = _mapGenerator.GetInteriorDesign().GetEnemies();
        if(Enemies.Count == 0) { throw new Exception("There are no enemies in this map."); }
    }

    public void Update()
    {
        for(int i = Events.Count - 1; i >= 0; i--)
        {
            if (Events[i].Duration < DateTime.Now) 
            {
                //Events.Remove(Events[i]);
            }
        }
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

    public void PlayerAttack(int x, int y)
    {
        Events.Add(new Event
        {
            Type = EventType.SlashAttack,
            X = x,
            Y = y,
            Duration = DateTime.Now.AddSeconds(1)
        });
        int px = Player.GetX();
        int py = Player.GetY();

        // Calculate distance (including diagonals)
        int dx = Math.Abs(x - px);
        int dy = Math.Abs(y - py);

        // If distance is <= 1 in both directions, it's a neighbor (including diagonals)
        // If you want to exclude diagonals, use: (dx + dy == 1)
        if (dx <= 1 && dy <= 1 && (dx != 0 || dy != 0))
        {
            var targetTile = Map[x][y];
        
            // Check if there is an enemy here
            if (targetTile.character != null)
            {
                Player.Attack(targetTile.character,10);
            }
        }
    }
}