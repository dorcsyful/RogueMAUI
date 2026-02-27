using System.Diagnostics;
using RogueCore.Models;
using RogueCore.Services.Algorithms;

namespace RogueCore.Entities;

public class Enemy : Character
{
    private Room _room;
    private float attackCooldown = 2f;
    private DateTime lastAttackTime = DateTime.MinValue;
    private DateTime _nextRetargetTime;
    private int _pathDirectionX = 0;
    private int _pathDirectionY = 0;
    public List<Tile>? plannedPath = new List<Tile>();
    public List<Tile>? plannedPath2 = new List<Tile>();
    private bool hasTargeted = false;
    private bool isAttacking = false;
    public Enemy(int x, int y, Room room) : base(x, y)
    {
        _health = 100;
        _room = room;
        _nextRetargetTime = DateTime.Now;
    }
    
    public void UpdateEnemy(float deltaTime, Player player, List<List<Tile>> map)
    {
        if (_isDead) return;

        if (NeighboringPlayer(player))
        {
            TimeAttack(player);
        }
        
        if (!hasTargeted && !_isMoving || (DateTime.Now > _nextRetargetTime))
        {
            //EmergencyStop();
            Retarget(player, map);
        }

        float inputX = 0;
        float inputY = 0;

        if (plannedPath != null && plannedPath.Count > 0)
        {
            while (plannedPath.Count > 0 && 
                   ((plannedPath[0].x == _x && plannedPath[0].y == _y) || 
                    (plannedPath[0].x == _targetX && plannedPath[0].y == _targetY && _isMoving)))
            {
                plannedPath.RemoveAt(0);
            }

            if (plannedPath.Count > 0)
            {
                // If moving, calculate from _targetX (where we will be in a millisecond)
                // If not moving, calculate from _x (where we are)
                int referenceX = _isMoving ? _targetX : _x;
                int referenceY = _isMoving ? _targetY : _y;

                inputX = plannedPath[0].x - referenceX;
                inputY = plannedPath[0].y - referenceY;
            }
        }

        Update(deltaTime, inputX, inputY, map);
    }
    
    private void NextTileDirection()
    {
        if (plannedPath == null || plannedPath.Count <= 0)
        {
            _pathDirectionX = 0;
            _pathDirectionY = 0;
            return;
        }
        
        int dx = plannedPath[0].x - _x;
        int dy = plannedPath[0].y - _y;

        if (dx != 0 && dy != 0)
        {
            _pathDirectionX = Math.Sign(dx);
            return;
        }

        _pathDirectionX = Math.Sign(dx);
        _pathDirectionY = Math.Sign(dy);
    }
    
    private void Retarget(Player player, List<List<Tile>> map)
    {
        if(_room.HasTile(map[player.GetX()][player.GetY()]))
        {
            hasTargeted = true;
            plannedPath = AStar.FindPath(map[_x][_y], map[player.GetX()][player.GetY()], map);
            plannedPath2 = AStar.FindPath(map[_x][_y], map[player.GetX()][player.GetY()], map);
            _nextRetargetTime = DateTime.Now.AddSeconds(1);
            NextTileDirection();
        }
    }
    
    private bool NeighboringPlayer(Player player)
    {
        if (player.GetX() == _x && Math.Abs(player.GetY() - _y) <= 1) return true;
        if(player.GetY() == _y && Math.Abs(player.GetX() - _x) <= 1) return true;
        return false;
    }
    
    public override void Die()
    {
            _isDead = true;
    }

    public List<Tile>? GetPlannedPath() => plannedPath;

    protected override void CheckTile(Tile tile)
    {
        
    }

    private void TimeAttack(Player player)
    {
        if(DateTime.Now > lastAttackTime.AddSeconds(attackCooldown) && !IsTakingDamage() && !player.IsDead())
        {
            // Attack logic here (e.g., reduce player health)
            lastAttackTime = DateTime.Now;
            Attack(player, 10);
            Console.WriteLine("Enemy attacked player");
            isAttacking = true;
        }
    }
    
    public void SetIsAttacking(bool attacking)
    {
        isAttacking = attacking;
    }
    public bool IsAttacking() { return isAttacking; }
}