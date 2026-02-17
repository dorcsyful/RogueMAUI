namespace RogueCore.Entities;

public abstract class Character
{
    protected int _x;
    protected int _y;
    protected int _maxHealth;
    public int _health;
    
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
    
    public int GetX() => _x;
    public int GetY() => _y;
}