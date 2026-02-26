using RogueCore.Models;

namespace RogueCore.Entities;

public abstract class Character(int x, int y, bool isPlayer)
{
    protected int _x = x;
    protected int _y = y;

    protected float _visual_x = x;
    protected float _visual_y = y;
    protected int _maxHealth;
    public int _health;
    
    protected bool _isMoving = false;
    protected float _moveProgress = 0f; 
    protected int _targetX, _targetY;
    protected int _directionX = 1, _directionY = 1;
    protected int frameCounter = 0;
    public float frameCounterProgress = 0f;
    public float MoveSpeed = 5.0f;
    public  bool IsPlayer;
    private bool _isTakingDamage = false;
    protected bool _isDead = false;
    protected bool isStuck = false;
    public void Move(int dx, int dy)
    {
        _x += dx;
        _y += dy;
    }
    
    public void SetPosition(int x, int y)
    {
        _x = x;
        _y = y;
    }
    
    public void AddHealth(int health)
    {
        health = Math.Clamp(_health + health, 0, _maxHealth);
    }

    public void ResetHealth()
    {
        _health = _maxHealth;
    }

    public abstract void Die();

    protected abstract void CheckTile(Tile tile);
    
    public void Update(float deltaTime, float inputX, float inputY, List<List<Tile>> map )
    {
        if (!_isMoving)
        {
            if (inputX != 0 || inputY != 0)
            {
                // Determine direction 
                int dx = inputX > 0 ? 1 : (inputX < 0 ? -1 : 0);
                int dy = inputY > 0 ? 1 : (inputY < 0 ? -1 : 0);
                StartMove(dx, dy, map);
            }
        }
        else
        {
            ContinueMove(deltaTime, inputX, inputY, map);
        }
        UpdateAnimation(deltaTime);
    }
    
    protected void StartMove(int dx, int dy, List<List<Tile>> map)
    {
        int nextX = _x + dx;
        int nextY = _y + dy;

        if (IsPathClear(nextX, nextY, map))
        {
            _targetX = nextX;
            _targetY = nextY;
            _isMoving = true;
            _moveProgress = 0f;
        }
        else
        {
            isStuck = true;
        }
    }

    protected bool IsPathClear(int targetX, int targetY, List<List<Tile>> map)
    {
        if (targetX < 0 || targetY < 0 || targetX >= map.Count || targetY >= map[0].Count) 
            return false;

        if (map[targetX][targetY].type == TileType.Empty ||
            (map[targetX][targetY].character != null && !map[targetX][targetY].character.IsPlayer))
            return false;

        if (targetX != _x && targetY != _y)
        {
            if (map[targetX][_y].type == TileType.Empty || map[_x][targetY].type == TileType.Empty)
                return false;
        }

        return true;
    }
    
    protected void ContinueMove(float deltaTime, float inputX, float inputY, List<List<Tile>> map)
    {
        if (_isDead) return;
        _moveProgress += deltaTime * MoveSpeed;
    
        if (Math.Sign(inputX) != 0) _directionX = Math.Sign(inputX);
        if (Math.Sign(inputY) != 0) _directionY = Math.Sign(inputY);

        if (_moveProgress >= 1.0f)
        {
            // Arrived at the target tile
            map[_x][_y].character = null;
            _x = _targetX;
            _y = _targetY;
            map[_x][_y].character = this;
            CheckTile(map[_targetX][_targetY]);

            float remainder = _moveProgress - 1.0f;

            // Use the FRESH inputs passed from UpdateEnemy
            int dx = (int)inputX;
            int dy = (int)inputY;

            bool isPathClear = IsPathClear(_x + dx, _y + dy, map);
        
            // Only carry momentum if there's a valid next tile to move to
            if ((dx != 0 || dy != 0) && isPathClear)
            {
                _targetX = _x + dx;
                _targetY = _y + dy;
                _moveProgress = remainder; // The magic "Smooth" line
                UpdateVisualPosition();
            }
            else
            {
                // Stop moving if the path is finished or blocked
                _isMoving = false;
                _moveProgress = 0f;
                _visual_x = _x;
                _visual_y = _y;
                isStuck = !isPathClear && (dx != 0 || dy != 0);
            }
        }
        else
        {
            UpdateVisualPosition();
        }
    }
    
    protected void EmergencyStop()
    {
        _isMoving = false;
        _moveProgress = 0f;
        _visual_x = _x;
        _visual_y = _y;
    }
    
    protected void UpdateVisualPosition()
    {
        _visual_x = _x + (_targetX - _x) * _moveProgress;
        _visual_y = _y + (_targetY - _y) * _moveProgress;
    }    
    
    protected void UpdateAnimation(float deltaTime)
    {
        if (_isMoving)
        {
            frameCounterProgress += deltaTime * MoveSpeed;
            if (frameCounterProgress >= 0.2f)
            {
                frameCounter = (frameCounter + 1) % 6;
                frameCounterProgress = 0f;
            }
        }
        else
        {
            frameCounter = -1; 
        }
    }
    
    public void TakeDamage(int damage)
    {
        _isTakingDamage = true;
        _health -= damage;
        if (_health <= 0)
        {
            Die();
        }
    }
    
    public void Attack(Character? target, int damage)
    {
        target?.TakeDamage(damage);
    }
    
    public bool IsDead() => _isDead;
    public bool IsTakingDamage() => _isTakingDamage;
    public void DisableDamage() => _isTakingDamage = false;
    public int GetX() => _x;
    public int GetY() => _y;
    public float GetVisualX() => _visual_x;
    public float GetVisualY() => _visual_y;
    public void SetVisualX(float x) => _visual_x = x;
    public void SetVisualY(float y) => _visual_y = y;
    public int GetDirectionX() => _directionX;
    public int GetDirectionY() => _directionY;
    public int GetActiveFrame() => frameCounter;
}