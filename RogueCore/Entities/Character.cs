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
    public float MoveSpeed = 1.0f; // Tiles per second
    
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

        if ((inputX != 0 || inputY != 0))
        {
            if(!_isMoving)  StartMove(nextTile);
            else
            {
                if (_moveProgress >= 0.8f) StartMove(nextTile); // Allow changing direction mid-move after halfway point
                else ContinueMove(deltaTime);
            }
        }
        else
        {
            if(_isMoving) ContinueMove(deltaTime);
        }
        
        // if (!_isMoving)
        // {
        //     // Only start moving if there is input and the target is walkable
        //     if (inputX != 0 || inputY != 0)
        //     {
        //         if(inputX != 0) _directionX = (int)Math.Sign(inputX);
        //         _directionY = (int)Math.Sign(inputY);
        //         StartMove(nextTile);
        //     }
        // }
        // else
        // {
        //     ContinueMove(deltaTime);
        // }
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
        if (_moveProgress > 0.8)
        {
            _x = _targetX;
            _y = _targetY;

        }
        if (_moveProgress >= 1.0f)
        {
            frameCounter = -1;
            // Movement complete: Snap to target
            _visual_x = _x;
            _visual_y = _y;
            _isMoving = false;
            _moveProgress = 0f;
        }
        else
        {
            if(frameCounterProgress > 0.016 *5) // 5 frames per tile at 60fps
            {
                frameCounterProgress = 0f;
                frameCounter = (frameCounter + 1) % 6; // Loop through 4 frames
            }
            else
            {
                frameCounterProgress += deltaTime;
            }
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
    public int GetDirectionX() => _directionX;
    public int GetDirectionY() => _directionY;
    public int GetActiveFrame() => frameCounter;
}