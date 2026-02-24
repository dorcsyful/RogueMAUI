using RogueMAUI.Services;

namespace RogueMAUI;
public class WindowsInputService : IInputService
{
    private float _x, _y;
    private (float X, float Y)? _pendingClick;
    private bool _attackTriggered;

    public void Initialize(Microsoft.UI.Xaml.Window window)
    {
        window.Content.KeyDown += (s, e) => {
            if (e.Key == Windows.System.VirtualKey.W) _y = -1;
            if (e.Key == Windows.System.VirtualKey.S) _y = 1;
            if (e.Key == Windows.System.VirtualKey.D) _x = 1;
            if (e.Key == Windows.System.VirtualKey.A) _x = -1;
            //TODO: Add more keys for actions like pause, etc.
            
        };
        window.Content.KeyUp += (s, e) => {
            // Reset movement vector when keys are released
            if (e.Key == Windows.System.VirtualKey.A || e.Key == Windows.System.VirtualKey.D) _x = 0;
            if (e.Key == Windows.System.VirtualKey.W || e.Key == Windows.System.VirtualKey.S) _y = 0;
        };
        
        window.Content.PointerPressed += (s, e) => 
        {
            var pt = e.GetCurrentPoint(window.Content);
            if (pt.Properties.IsLeftButtonPressed)
            {
                _pendingClick = ((float)pt.Position.X, (float)pt.Position.Y);
            }
        };    }

    public (float X, float Y) GetMovementVector() => (_x, _y);

    public (float X, float Y)? GetAttackVector()
    {
        var click = _pendingClick;
        _pendingClick = null; // Consume the click
        return click;
        
    }
    
}