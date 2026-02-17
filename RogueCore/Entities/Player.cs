using RogueCore.Helpers;

namespace RogueCore.Entities;

public class Player : Character
{
    public int numOfCoins;
    public Player()
    {
        _maxHealth = GameSettings.Player.MaxHealth;
        _health = _maxHealth;
    }

    public void AddCoins(int amount)
    {
        numOfCoins += amount;
    }

    public override void Die()
    {
        throw new NotImplementedException();
    }
}