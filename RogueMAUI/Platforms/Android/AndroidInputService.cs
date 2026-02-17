using RogueMAUI.Services;

namespace RogueMAUI;
public class AndroidInputService : IInputService
{
    public float JoystickX, JoystickY;
    public (float X, float Y) GetMovementVector() => (JoystickX, JoystickY);
}