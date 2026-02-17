using RogueCore.Helpers;
using RogueCore.Models;

namespace RogueCore.Services.Dungeon;

public class InteriorDesign
{
    private Random _random;
    private List<List<Tile>> _map;
    private List<Room> _rooms;
    private Tile _entrance;
    private Tile _exit;

    public InteriorDesign(Random random)
    { 
        _random = random;
    }

    public void AssignMapRoom(List<List<Tile>> map, List<Room> rooms)
    {
        _map = map;
        _rooms = rooms;
    }

    public Tile GetEntrance()
    {
        return _entrance;
    }

    public Tile GetExit()
    {
        return _exit;
    }
    
    public void CreateEntranceExit()
    {
        Room? start = null;
        Room? end = null;
        int distance = 0;
        //Find the two furthest rooms and place entrance and exit there
        foreach (Room outer in _rooms)
        {
            foreach (Room inner in _rooms)
            {
                if (GetDistance(outer, inner) >= distance)
                {
                    start = outer;
                    end = inner;
                    distance = (int)GetDistance(outer, inner);
                }
            }
        }
        if (start != null && end != null)
        {
            // Place entrance and exit in the middle of the rooms
            int startX = (start.x1 + start.x2) / 2;
            int startY = (start.y1 + start.y2) / 2;
            int endX = (end.x1 + end.x2) / 2;
            int endY = (end.y1 + end.y2) / 2;
            Tile entrance = new Tile(startX, startY, Models.TileType.Entrance);
            Tile exit = new Tile(endX, endY, Models.TileType.Exit);
            _map[startX][startY] = entrance;
            _map[endX][endY] = exit;
            _entrance = entrance;
            _exit = exit;
            Console.WriteLine("ENTRANCE PLACED: " + entrance.x + "," + entrance.y);
            Console.WriteLine("EXIT PLACED: " + exit.x + "," + exit.y);
        }
    }

    public void PlaceCoins()
    {
        int min = Math.Min(GameSettings.Interior.MaxNumOfCoins,
            GameSettings.Interior.NumOfCoinsPerRoom * _rooms.Count);
        int max = Math.Max(GameSettings.Interior.MaxNumOfCoins,
            GameSettings.Interior.NumOfCoinsPerRoom * _rooms.Count);
        
        int numOfCoins = _random.Next(min, max) / _rooms.Count;

        foreach (Room room in _rooms)
        {
            for (int i = 0; i < numOfCoins; i++)
            {
                int x = _random.Next(room.x1, room.x2);
                int y = _random.Next(room.y1, room.y2);
                if (_map[x][y].type == Models.TileType.Floor)
                {
                    _map[x][y] = new Tile(x, y, Models.TileType.Coin);
                }
                else
                {
                    while (_map[x][y].type != TileType.Floor)
                    {
                        x = _random.Next(room.x1, room.x2);
                        y = _random.Next(room.y1, room.y2);
                    }

                    _map[x][y] = new Tile(x, y, Models.TileType.Coin);
                }
            }
        }
    }
    
    private static float GetDistance(Room a, Room b)
    {
        int ax = (a.x1 + a.x2) / 2;
        int ay = (a.y1 + a.y2) / 2;
        int bx = (b.x1 + b.x2) / 2;
        int by = (b.y1 + b.y2) / 2;

        return (float)Math.Sqrt(Math.Pow(ax - bx, 2) + Math.Pow(ay - by, 2));
    }
}