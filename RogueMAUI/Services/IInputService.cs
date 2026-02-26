namespace RogueMAUI.Services;

public interface IInputService
{
    (float X, float Y) GetMovementVector();
    (float X, float Y)? GetAttackVector();
    (float X, float Y)? GetMenuClick();
}