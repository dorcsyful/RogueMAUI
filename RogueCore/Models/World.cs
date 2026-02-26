using RogueCore.Entities;
using RogueCore.Helpers;
using RogueCore.Services.Dungeon;

namespace RogueCore.Models;

public class World
{
    public enum GameState
    {
        Playing,
        GameOver,
        NextLevel
    }
    
    public List<List<Tile>> Map;
    public List<Room> Rooms;
    public List<Corridor> Corridors;
    public Player Player;
    public List<Enemy> Enemies;
    private MapGenerator _mapGenerator;
    public List<Event> Events { get; private set; }
    public GameState State { get; private set; }

    public World()
    {
        Events = new List<Event>();
        State = GameState.GameOver;
        _mapGenerator = new MapGenerator();
        _mapGenerator.GenerateMap();
        Map = _mapGenerator.GetMap();
        Rooms = _mapGenerator.GetRooms();
        Corridors = _mapGenerator.GetCorridors();
        Player = new Player(_mapGenerator.GetEntranceCoordinates().x, _mapGenerator.GetEntranceCoordinates().y);
        Map[Player.GetX()][Player.GetY()].character = Player;
        Player.SetPosition(_mapGenerator.GetInteriorDesign().GetEntrance().x, _mapGenerator.GetInteriorDesign().GetEntrance().y);
        Enemies = _mapGenerator.GetInteriorDesign().GetEnemies();
        if (Enemies.Count == 0)
        {
            throw new Exception("There are no enemies in this map.");
        }
    }

    public void Reset()
    {
        
    }
    
    public void Update()
    {
        for (int i = 0; i < Enemies.Count; i++)
        {
            Enemies[i].UpdateEnemy(0.016f, Player, Map);
            if (Enemies[i].IsAttacking())
            {
                Events.Add(new Event
                {
                    Type = EventType.SlashAttack,
                    X = Player.GetX(),
                    Y = Player.GetY(),
                    LifespanSeconds = 1f,
                    Source = Enemies[i]
                });
                Enemies[i].SetIsAttacking(false);

            }
        }
        
        for(int i = Events.Count - 1; i >= 0; i--)
        {
            var elapsed = (DateTime.Now - Events[i].StartTime).TotalSeconds;
            if (elapsed >= Events[i].LifespanSeconds) 
            {
                Character? character = Map[(int)Events[i].X][(int)Events[i].Y].character;
                Events[i].RemoveEffect(character);
                if (Events[i].Type == EventType.SlashAttack && character != null && character.IsDead())
                {
                    if (!character.IsPlayer)
                    {
                        EnemyDeath(i, character);
                    }
                    else
                    {
                        State = GameState.GameOver;
                        Events.Add(new Event
                        {
                            Type = EventType.ExplosionDeath,
                            X = Events[i].X,
                            Y = Events[i].Y,
                            LifespanSeconds = 1f,
                            Source = Player
                        });
                    }
                }

                if (Events[i].Type == EventType.ExplosionDeath)
                {
                    Map[(int)Events[i].X][(int)Events[i].Y].type = TileType.Coin;
                }
                Events.RemoveAt(i);
            }
        }
    }

    private void EnemyDeath(int i, Character character)
    {
        Events.Add(new Event
        {
            Type = EventType.ExplosionDeath,
            X = Events[i].X,
            Y = Events[i].Y,
            LifespanSeconds = 1f,
            Source = Events[i].Source
        });
        Enemies.Remove((Enemy)character);
        Map[(int)Events[i].X][(int)Events[i].Y].character = null;
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
                LifespanSeconds = 1f,
                Source = Player
        });

            var targetTile = Map[x][y];
        
            // Check if there is an enemy here
            if (targetTile.character != null)
            {
                Player.Attack(targetTile.character,10);
            }
    }
}