using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using RogueCore.Input;
using RogueCore.Models;
using RogueCore.Services.Dungeon;
using RogueMAUI.Services;

namespace RogueMAUI.ViewModels;

public class GameViewModel
{
    public World CurrentWorld { get; private set; }
    public IInputService InputService { get; }
    public GameViewModel(IInputService inputService)
    {
        Initialize();    
        InputService = inputService;
    }
    
    public void Update()
    {
        var (x, y) = InputService.GetMovementVector();
        if (x != 0 || y != 0)
        {
            int directionX = (int)Math.Sign(x);
            int directionY = (int)Math.Sign(y);
            CurrentWorld.TryMovePlayer(directionX, directionY);
        }
    }
    
    public void Initialize()
    {
        CurrentWorld = new World();
    }

    
}