using RogueCore.Entities;
using RogueCore.Helpers;
using RogueCore.Models;

namespace RogueCore.Services.Dungeon;

public class InteriorDesign
{
    private Random _random;
    private List<List<Tile>> _map;
    private List<Room> _rooms;
    private List<Enemy> _enemies;
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

        end = start;
        if (start != null && end != null)
        {
            // Place entrance and exit in the middle of the rooms
            int startX = (start.x1 + start.x2) / 2;
            int startY = (start.y1 + start.y2) / 2;
            int endX = (end.x1 + end.x2) / 2;
            int endY = (end.y1 + end.y2) / 2;
            Tile entrance = new Tile(startX, startY, Models.TileType.Entrance);
            Tile exit = new Tile(endX + 1, endY, Models.TileType.Exit);
            _map[startX][startY] = entrance;
            _map[endX + 1][endY] = exit;
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
    
    public void PlaceHealthPotions()
    {
        int min = Math.Min(GameSettings.Interior.MaxPotionPerRoom,
            GameSettings.Interior.NumOfPotionPerRoom * _rooms.Count);
        int max = Math.Max(GameSettings.Interior.MaxPotionPerRoom,
            GameSettings.Interior.NumOfPotionPerRoom * _rooms.Count);
        
        int numOfPotion = Math.Max(1, _random.Next(min, max) / _rooms.Count);

        foreach (Room room in _rooms)
        {
            for (int i = 0; i < numOfPotion; i++)
            {
                int x = _random.Next(room.x1, room.x2);
                int y = _random.Next(room.y1, room.y2);
                if (_map[x][y].type == Models.TileType.Floor)
                {
                    _map[x][y] = new Tile(x, y, Models.TileType.HealthPotion);
                }
                else
                {
                    while (_map[x][y].type != TileType.Floor)
                    {
                        x = _random.Next(room.x1, room.x2);
                        y = _random.Next(room.y1, room.y2);
                    }

                    _map[x][y] = new Tile(x, y, Models.TileType.HealthPotion);
                }
            }
        }
    }
    
    
    public void PlaceChasingEnemies()
    {
        int min = Math.Min(GameSettings.Interior.MaxChasingEnemiesPerRoom,
            GameSettings.Interior.MaxNumOfEnemies * _rooms.Count);
        int max = Math.Max(GameSettings.Interior.MaxChasingEnemiesPerRoom,
            GameSettings.Interior.MaxNumOfEnemies * _rooms.Count);
        
        int numOfEnemy = Math.Max(1, _random.Next(min, max) / _rooms.Count);
        _enemies = new List<Enemy>();
        foreach (Room room in _rooms)
        {
            for (int i = 0; i < numOfEnemy; i++)
            {
                int tries = 0;
                int x;
                int y;
                bool checkTileForEnemyPlacement;
                do
                {
                    x = _random.Next(room.x1, room.x2);
                    y = _random.Next(room.y1, room.y2);
                    checkTileForEnemyPlacement = CheckTileForEnemyPlacement(x, y, room);
                    if (checkTileForEnemyPlacement)
                    {
                        _enemies.Add(new Enemy(x, y, room));
                        _map[x][y].character = _enemies.Last();

                        break;
                    }
                    else
                    {
                        
                        tries++;

                    }
                } while (!checkTileForEnemyPlacement && tries <= room.Size());

            }
        }

    }

    private bool CheckTileForEnemyPlacement(int x, int y, Room room)
    {
        return NeighbourEmpty(x,y) &&
                !ContainsPlayer(Math.Clamp(x - 4, room.x1, room.x2), Math.Clamp(y - 4, room.y1, room.y2),
                    Math.Clamp(x + 4, room.x1, room.x2), Math.Clamp(y + 4, room.y1, room.y2), _entrance.x, _entrance.y);
    }
    
    private bool NeighbourEmpty(int x, int y)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (x + i >= 0 && x + i < _map.Count && y + j >= 0 && y + j < _map[0].Count)
                {
                    if (_map[x + i][y + j].character != null)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    private bool ContainsPlayer(int x1, int y1, int x2, int y2, int playerX, int playerY)
    {
        return playerX >= x1 && playerX <= x2 && playerY >= y1 && playerY <= y2;
    }
    
    public List<Enemy> GetEnemies() => _enemies;
    
    private static float GetDistance(Room a, Room b)
    {
        int ax = (a.x1 + a.x2) / 2;
        int ay = (a.y1 + a.y2) / 2;
        int bx = (b.x1 + b.x2) / 2;
        int by = (b.y1 + b.y2) / 2;

        return (float)Math.Sqrt(Math.Pow(ax - bx, 2) + Math.Pow(ay - by, 2));
    }
}