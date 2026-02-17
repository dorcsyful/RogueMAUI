using RogueMAUI.Services;

namespace RogueMAUI;
public class WindowsInputService : IInputService
{
    private float _x, _y;

    public void Initialize(Microsoft.UI.Xaml.Window window)
    {
        window.Content.KeyDown += (s, e) => {
            if (e.Key == Windows.System.VirtualKey.W) _y = -1;
            if (e.Key == Windows.System.VirtualKey.S) _y = 1;
            if (e.Key == Windows.System.VirtualKey.D) _x = 1;
            if (e.Key == Windows.System.VirtualKey.A) _x = -1;
            //TODO: Add more keys for actions like attack, interact, pause, etc.
            
        };
        window.Content.KeyUp += (s, e) => {
            // Reset movement vector when keys are released
            if (e.Key == Windows.System.VirtualKey.A || e.Key == Windows.System.VirtualKey.D) _x = 0;
            if (e.Key == Windows.System.VirtualKey.W || e.Key == Windows.System.VirtualKey.S) _y = 0;
        };
    }

    public (float X, float Y) GetMovementVector() => (_x, _y);
}