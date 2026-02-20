using System;
using RogueCore.Models;
using RogueMAUI.Services;

namespace RogueMAUI.ViewModels;

public class GameViewModel
{
    public World CurrentWorld { get; private set; }
    public IInputService InputService { get; }
    
    public float CameraX { get; private set; }
    public float CameraY { get; private set; }
    
    
    
    public GameViewModel(IInputService inputService)
    {
        Initialize();    
        InputService = inputService;
        CurrentWorld = new World();
    }
    
    public void Update()
    {

        var (x, y) = InputService.GetMovementVector();
        CurrentWorld.Player.Update(0.016f,x, y, CurrentWorld.Map[(int)(CurrentWorld.Player.GetX() + x)][
            (int)(CurrentWorld.Player.GetY() + y)]);
        // 3. Camera Follow
        float targetCameraX = CurrentWorld.Player.GetVisualX() - 3.5f; // 3.5 centers the 16px player better in an 8-tile view
        float targetCameraY = CurrentWorld.Player.GetVisualY() - 3.5f;

        float cameraLerpSpeed = 0.2f; // Camera should be slightly "lazier" than player
        CameraX += (targetCameraX - CameraX) * cameraLerpSpeed;
        CameraY += (targetCameraY - CameraY) * cameraLerpSpeed;
    }
    
    public void Initialize()
    {
        CurrentWorld = new World();
    }

}