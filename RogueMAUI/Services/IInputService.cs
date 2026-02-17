namespace RogueMAUI.Services;

public interface IInputService
{
    (float X, float Y) GetMovementVector();
}