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
    
    public void Update(float deltaTime, float inputX, float inputY, Tile nextTile)
    {
        if (!_isMoving)
        {
            // Only start moving if there is input and the target is walkable
            if (inputX != 0 || inputY != 0)
            {
                StartMove(nextTile);
            }
        }
        else
        {
            ContinueMove(deltaTime);
        }
    }

    private void StartMove(Tile nextTile)
    {
        if (nextTile.type != TileType.Empty)
        {
            _targetX = nextTile.x;
            _targetY = nextTile.y;
            _isMoving = true;
            _moveProgress = 0f;
        }
    }

    private void ContinueMove(float deltaTime)
    {
        _moveProgress += deltaTime * MoveSpeed;

        if (_moveProgress >= 1.0f)
        {
            // Movement complete: Snap to target
            _x = _targetX;
            _y = _targetY;
            _visual_x = _x;
            _visual_y = _y;
            _isMoving = false;
            _moveProgress = 0f;
        }
        else
        {
            // Interpolate Visual Position: LERP(start, end, progress)
            _visual_x = _x + (_targetX - _x) * _moveProgress;
            _visual_y = _y + (_targetY - _y) * _moveProgress;
        }
    }
    
    
    public int GetX() => _x;
    public int GetY() => _y;
    public float GetVisualX() => _visual_x;
    public float GetVisualY() => _visual_y;
    public void SetVisualX(float x) => _visual_x = x;
    public void SetVisualY(float y) => _visual_y = y;
}