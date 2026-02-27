using System.ComponentModel;
using System.Runtime.CompilerServices;
using RogueCore.Models;
using RogueMAUI.Graphics;
using RogueMAUI.Services;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace RogueMAUI.ViewModels;

public class GameViewModel : INotifyPropertyChanged
{
    private static class SpritePaints 
    {
        public static readonly SKPaint Default = new()
        {
            Color = SKColors.Black,
        };
    
        public static readonly SKPaint RedTint = new()
        { 
            ColorFilter = SKColorFilter.CreateBlendMode(new SKColor(255, 0, 0, 128), SKBlendMode.SrcATop)
        };
    }
    
    public World CurrentWorld { get; private set; }
    public IInputService InputService { get; }
    private List<MenuButton> _menuButtons;
    public float CameraX { get; private set; }
    public float CameraY { get; private set; }
    public bool QuitRequested { get; set; }
    public int Health
    {
        get => CurrentWorld.Player._health;
        set
        {
            CurrentWorld.Player._health = value;
            OnPropertyChanged(); // Tell the UI to update!
        }
    }

    public int CoinCount
    {
        get => CurrentWorld.Player.numOfCoins;
        set
        {
            CurrentWorld.Player.numOfCoins = value;
            OnPropertyChanged();
        }
    }

    private float tx, ty, scale;
    private SKBitmap? _tileSheet;
    private float _viewLeft;
    private float _viewTop;


    public GameViewModel(IInputService inputService)
    {
        Initialize();    
        InputService = inputService;
        CurrentWorld = new World();
        LoadAssets();
        _menuButtons = new List<MenuButton>
        {
            new MenuButton("Restart Game", () => StartGame()),
            new MenuButton("Exit", () => QuitGame())
        };
    }

    private void StartGame()
    {
        Console.WriteLine("Starting game");
        int health = CurrentWorld.Player._health;
        int coinCount = CurrentWorld.Player.numOfCoins;
        CurrentWorld = new World();
        CurrentWorld.Player._health = health;
        CurrentWorld.Player.numOfCoins = coinCount;
        OnPropertyChanged(nameof(CurrentWorld));
    }

    private void QuitGame()
    {
        QuitRequested = true;
    }

    public void Initialize()
    {
        CurrentWorld = new World();
    }
    
    private void LoadAssets()
    {
        using var stream = FileSystem.OpenAppPackageFileAsync("tilemap.png").Result;
        _tileSheet = SKBitmap.Decode(stream);
    }
    
    public void Update()
    {
        if(CurrentWorld.State == World.GameState.GameOver)
        {
            HandleMenuClick();
            return;
        }

        if (CurrentWorld.State == World.GameState.NextLevel)
        {
            return;
        }

        Health = CurrentWorld.Player._health;
        CoinCount = CurrentWorld.Player.numOfCoins;
        var (x, y) = InputService.GetMovementVector();
        CurrentWorld.Player.Update(0.016f,x, y, CurrentWorld.Map);
        float targetCameraX = CurrentWorld.Player.GetVisualX() - 3.5f; // 3.5 centers the 16px player better in an 8-tile view
        float targetCameraY = CurrentWorld.Player.GetVisualY() - 3.5f;

        float cameraLerpSpeed = 0.2f; // Camera should be slightly "lazier" than player
        CameraX += (targetCameraX - CameraX) * cameraLerpSpeed;
        CameraY += (targetCameraY - CameraY) * cameraLerpSpeed;
        CurrentWorld.Update();
        
        ProcessAttackInput();
    }

    public void HandleMenuClick()
    {
        var click = InputService.GetMenuClick();
        if (click == null ) return;
        float localX = click.Value.X - tx;
        float localY = click.Value.Y - ty;

        float gameX = localX / scale;
        float gameY = localY / scale;

        float worldPixelX = gameX + _viewLeft;
        float worldPixelY = gameY + _viewTop;

        foreach (var btn in _menuButtons)
        {
            if (btn.Contains(worldPixelX, worldPixelY))
            {
                btn.OnClick();
                break; 
            }
        }
    }
    
    private void ProcessAttackInput()
    {
        var attackVector = InputService.GetAttackVector();
        if (attackVector.HasValue)
        {
            var click = attackVector;

            float localX = click.Value.X - tx;
            float localY = click.Value.Y - ty;

            float gameX = localX / scale;
            float gameY = localY / scale;

            float worldPixelX = gameX + _viewLeft;
            float worldPixelY = gameY + _viewTop;

            int clickedTileX = (int)Math.Floor(worldPixelX / 16.0f);
            int clickedTileY = (int)Math.Floor(worldPixelY / 16.0f);

            if (CheckIfNeighbor(clickedTileX, clickedTileY))
            {
                CurrentWorld.PlayerAttack(clickedTileX, clickedTileY);
                
            }
        }
    }

    public void Draw(SKPaintSurfaceEventArgs e)
    {
        try
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
            
            canvas.Clear(SKColors.Black);

            var paint = new SKPaint();
            var world = CurrentWorld;

            // (128x128 pixels = 8x8 tiles)
            float viewWidth = 128.0f;
            float viewHeight = 128.0f;
            
            float viewCenterX = world.Player.GetVisualX() * 16.0f;
            float viewCenterY = world.Player.GetVisualY() * 16.0f;

            // Calculate top-left corner of view window
            _viewLeft = viewCenterX - viewWidth / 2.0f;
            _viewTop = viewCenterY - viewHeight / 2.0f;
            
            float canvasWidth = info.Width;
            float canvasHeight = info.Height;
            scale = Math.Min(canvasWidth / viewWidth, canvasHeight / viewHeight);

            AdjustCanvas(viewWidth, canvasWidth, canvasHeight, canvas, _viewLeft, _viewTop);

            // Calculate which tiles are visible in the view window
            int startX = Math.Max(0, (int)Math.Floor(_viewLeft / 16.0f));
            int startY = Math.Max(0, (int)Math.Floor(_viewTop / 16.0f));
            int endX = Math.Min(100, (int)Math.Ceiling((_viewLeft + viewWidth) / 16.0f));
            int endY = Math.Min(100, (int)Math.Ceiling((_viewTop + viewHeight) / 16.0f));
            
            DrawTiles(startX, endX, startY, endY, world, canvas, paint);
            DrawEnemies(world, startX, endX, startY, endY, canvas);

            DrawPlayer(world, _viewLeft, _viewTop, canvas);
            
            DrawEvents(world, startX, endX, startY, endY, canvas);
            DrawMenu(canvas, _viewLeft, _viewTop, viewWidth, viewHeight);
            canvas.Restore();

        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Paint error: {ex.Message}");
        }
    }

    private void AdjustCanvas(float viewWidth, float canvasWidth, float canvasHeight, SKCanvas canvas, float viewLeft,
        float viewTop)
    {
        // Calculate offsets to center the scaled view on canvas
        float scaledViewSize = viewWidth * scale;
        tx = (canvasWidth - scaledViewSize) / 2.0f;
        ty = (canvasHeight - scaledViewSize) / 2.0f;
        float gameSize = 128f * scale;
        SKRect gameRect = SKRect.Create(tx, ty, gameSize, gameSize);
        // Apply transformations: translate to center, scale up, then translate to view position
        canvas.Save(); // Save state before clipping/translating
        canvas.ClipRect(gameRect);
        canvas.Translate(tx, ty);
        canvas.Scale(scale);
        canvas.Translate(-viewLeft, -viewTop);
    }

    private void DrawEvents(World world, int startX, int endX, int startY, int endY, SKCanvas canvas)
    {
        for(int i = 0; i < world.Events.Count; i++)
        {
            var current = world.Events[i];
            if(current.X < startX  || current.X > endX || current.Y < startY || current.Y > endY)
            {
                continue; 
            }

            var animation = TileCoordinates.GetEventCoordinates(current.Type);
            var frame = (DateTime.Now - current.StartTime).TotalSeconds / 1.0 * animation.Length; // Assuming 1 second duration for full animation
            int frameIndex = Math.Min(animation.Length - 1, (int)frame);
            var eCoords = animation[frameIndex];
            var eSrc = new SKRect(eCoords.x, eCoords.y, eCoords.x + 16, eCoords.y + 16);
            float eLeft = (current.X * 16);
            float eTop = (current.Y * 16);
            var eDest = new SKRect(eLeft, eTop, eLeft + 16.0f, eTop + 16.0f);

            var character = world.Events[i].Source != null ? world.Events[i].Source : world.Player;
            if (character != null)
            {
                var directionX = Math.Sign(character.GetX() - current.X) == 0 ? 1 : Math.Sign(character.GetX() - current.X); // Default to facing right if perfectly vertical
                var directionY = Math.Sign(character.GetY() - current.Y); // Default to facing down if perfectly horizontal
                float rotation = 0;
                if (directionY != 0)
                {
                    rotation = directionY * 90f;
                }
                DrawSprite(canvas, eSrc, eDest, SpritePaints.Default,rotation,-directionX);
            }
        }
    }
    
    private void DrawTiles(int startX, int endX, int startY, int endY, World world, SKCanvas canvas, SKPaint paint)
    {
        // Only render visible tiles
        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                var tile = world.Map[x][y];
                int[] baseCoords = TileCoordinates.GetTileBaseCoordinates(tile.type);
                int[] decorCoords = TileCoordinates.GetDecorationTileCoordinates(tile.type);

                var dest = new SKRect(
                    x * 16.0f,
                    y * 16.0f,
                    (x + 1) * 16.0f,
                    (y + 1) * 16.0f
                );

                var src = new SKRect(baseCoords[0], baseCoords[1], baseCoords[0] + 16, baseCoords[1] + 16);
                var srcdecor = new SKRect(decorCoords[0], decorCoords[1], decorCoords[0] + 16, decorCoords[1] + 16);
                canvas.DrawBitmap(_tileSheet, src, dest, paint);
                canvas.DrawBitmap(_tileSheet, srcdecor, dest, paint);
            }
        }
    }

    private void DrawEnemies(World world, int startX, int endX, int startY, int endY, SKCanvas canvas)
    {
        for(int i = 0; i < world.Enemies.Count; i++)
        {
            var enemy = world.Enemies[i];

            if(enemy.GetVisualX() < startX  || enemy.GetVisualX() > endX || enemy.GetVisualY() < startY || enemy.GetVisualY() > endY)
            {
                continue; // Skip rendering this enemy if it's outside the view
            }
            var eCoords = enemy.GetActiveFrame() == -1 ? TileCoordinates.Enemy.Idle : TileCoordinates.Enemy.RunAnimation
                [enemy.GetActiveFrame()];
            var eSrc = new SKRect(eCoords.x, eCoords.y, eCoords.x + 16, eCoords.y + 16);
            float eLeft = (enemy.GetVisualX() * 16);
            float eTop = (enemy.GetVisualY() * 16);
            var eDest = new SKRect(eLeft, eTop, eLeft + 16.0f, eTop + 16.0f);
                
            var directionX = enemy.GetDirectionX();


            DrawSprite(canvas, eSrc, eDest,enemy.IsTakingDamage() ? SpritePaints.RedTint : SpritePaints.Default,0,directionX);
        }
    }

    private void DrawPlayer(World world, float viewLeft, float viewTop, SKCanvas canvas)
    {
        if (world.Player.IsDead()) return;
        var pCoords = world.Player.GetActiveFrame() == -1 ? TileCoordinates.Player.Idle : TileCoordinates.Player.RunAnimation
            [world.Player.GetActiveFrame()];
        
        var pSrc = new SKRect(pCoords.x, pCoords.y, pCoords.x + 16, pCoords.y + 16);
        float left = viewLeft + 4 * 16;
        float f = viewLeft + 16 * 4;
        var pDest = new SKRect(left, viewTop + 16 * 4, f + 16.0f, viewTop + 16 * 4 + 16.0f);
        DrawSprite(canvas, pSrc, pDest,world.Player.IsTakingDamage() ? SpritePaints.RedTint : SpritePaints.Default,0,world.Player.GetDirectionX());
    }

    private void DrawSprite(SKCanvas canvas, SKRect sourceRect,  SKRect destination,SKPaint paint, float rotationDegrees = 0, float flipX = 1,float flipY = 1)
    {
        canvas.Save();

        float width = sourceRect.Width;
        float height = sourceRect.Height;
        canvas.Translate(destination.Left + width / 2, destination.Top + height / 2);

        if (rotationDegrees != 0)
        {
            canvas.RotateDegrees(rotationDegrees);
        }

        canvas.Scale(flipX, flipY);
        

        SKRect destRect = new SKRect(-width / 2, -height / 2, width / 2, height / 2);
        canvas.DrawBitmap(_tileSheet, sourceRect, destRect, paint);

        canvas.Restore();
    }

    private void DrawMenu(SKCanvas canvas, float viewLeft, float viewTop, float width, float height)
    {
        if (CurrentWorld.State == World.GameState.Playing) return;
        var textPaint = new SKPaint { 
            Color = SKColors.White, 
            TextSize = height * 0.08f, // Text size is 8% of screen height
            IsAntialias = false,
            TextAlign = SKTextAlign.Center 
        };

        var boxPaint = new SKPaint { 
            Color = new SKColor(60, 60, 60), 
            Style = SKPaintStyle.Fill 
        };

        float centerX = viewLeft + width / 2f;
        float startY = viewTop + height * 0.3f; // Start at 30% down the screen
        float verticalPadding = height * 0.05f; // Gap between elements

        // 1. Draw Title
        canvas.DrawText("Game Over", centerX, startY, textPaint);

        // 2. Calculate Button Layout
        float buttonWidth = width * 0.6f; // Buttons take up 60% of canvas width
        float buttonHeight = height * 0.12f; // Each button is 12% of canvas height
        float currentY = startY + verticalPadding;

        foreach (var btn in _menuButtons)
        {
            // Define the button rectangle centered on X
            btn.Bounds = SKRect.Create(
                centerX - (buttonWidth / 2f), 
                currentY, 
                buttonWidth, 
                buttonHeight
            );

            // Draw the background
            canvas.DrawRect(btn.Bounds, boxPaint);

            // Draw text centered inside the calculated Bounds
            // We add buttonHeight * 0.7f to roughly baseline-center the text
            canvas.DrawText(
                btn.Text, 
                centerX, 
                currentY + (buttonHeight * 0.7f), 
                textPaint
            );

            // Move the cursor down for the next button
            currentY += buttonHeight + (verticalPadding * 0.5f);
        }
    }    
    
    private bool CheckIfNeighbor(int x, int y)
    {
        if(x == CurrentWorld.Player.GetX() && Math.Abs(y - CurrentWorld.Player.GetY()) == 1)
        {
            return true;
        }

        if (y == CurrentWorld.Player.GetY() && Math.Abs(x - CurrentWorld.Player.GetX()) == 1)
        {
            return true;
        }

        return false;
    }
    
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}