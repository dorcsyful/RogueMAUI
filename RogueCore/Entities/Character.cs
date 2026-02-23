using RogueCore.Models;

namespace RogueCore.Entities;

public abstract class Character
{
    protected int _x;
    protected int _y;

    protected float _visual_x;
    protected float _visual_y;
    protected int _maxHealth;
    public int _health;
    
    private bool _isMoving = false;
    private float _moveProgress = 0f; // 0.0 to 1.0
    private int _targetX, _targetY;
    private int _directionX = 1, _directionY = 1;
    private int frameCounter = 0;
    public float frameCounterProgress = 0f;
    public float MoveSpeed = 5.0f; // Tiles per second
    
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
    
    public void Update(float deltaTime, float inputX, float inputY, List<List<Tile>> map )
    {
        if (!_isMoving)
        {
            if (inputX != 0 || inputY != 0)
            {
                // Determine direction 
                int dx = inputX > 0 ? 1 : (inputX < 0 ? -1 : 0);
                int dy = inputY > 0 ? 1 : (inputY < 0 ? -1 : 0);
                _directionX = dx != 0 ? Math.Sign(dx) : _directionX;
                _directionY = dy != 0 ? Math.Sign(dy) : _directionY;
                StartMove(dx, dy, map);
            }
        }
        else
        {
            ContinueMove(deltaTime, inputX, inputY, map);
        }
        UpdateAnimation(deltaTime);
    }
    private void StartMove(int dx, int dy, List<List<Tile>> map)
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
    }

    private bool IsPathClear(int targetX, int targetY, List<List<Tile>> map)
    {
        if (targetX < 0 || targetY < 0 || targetX >= map.Count || targetY >= map[0].Count) 
            return false;

        if (map[targetX][targetY].type == TileType.Empty) 
            return false;

        if (targetX != _x && targetY != _y)
        {
            if (map[targetX][_y].type == TileType.Empty || map[_x][targetY].type == TileType.Empty)
                return false;
        }

        return true;
    }
    private void ContinueMove(float deltaTime, float inputX, float inputY, List<List<Tile>> map)
    {
        _moveProgress += deltaTime * MoveSpeed;

        if (_moveProgress >= 1.0f)
        {
            _x = _targetX;
            _y = _targetY;
            float remainder = _moveProgress - 1.0f;

            int dx = inputX > 0 ? 1 : (inputX < 0 ? -1 : 0);
            int dy = inputY > 0 ? 1 : (inputY < 0 ? -1 : 0);

            if ((dx != 0 || dy != 0) && IsPathClear(_x + dx, _y + dy, map))
            {
                _targetX = _x + dx;
                _targetY = _y + dy;
                _moveProgress = remainder; // Carry the momentum!
                UpdateVisualPosition();
            }
            else
            {
                _isMoving = false;
                _moveProgress = 0f;
                _visual_x = _x;
                _visual_y = _y;
            }
        }
        else
        {
            UpdateVisualPosition();
        }
    }
    private void UpdateVisualPosition()
    {
        _visual_x = _x + (_targetX - _x) * _moveProgress;
        _visual_y = _y + (_targetY - _y) * _moveProgress;
    }    
    
    private void UpdateAnimation(float deltaTime)
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