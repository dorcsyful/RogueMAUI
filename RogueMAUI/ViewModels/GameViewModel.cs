using System;
using RogueCore.Models;
using RogueMAUI.Services;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace RogueMAUI.ViewModels;

public class GameViewModel
{
    public World CurrentWorld { get; private set; }
    public IInputService InputService { get; }
    
    public float CameraX { get; private set; }
    public float CameraY { get; private set; }
    private float tx, ty, scale;
    private SKBitmap _tileSheet;
    private float _viewLeft;
    private float _viewTop;


    public GameViewModel(IInputService inputService)
    {
        Initialize();    
        InputService = inputService;
        CurrentWorld = new World();
        LoadAssets();
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
        var (x, y) = InputService.GetMovementVector();
        CurrentWorld.Player.Update(0.016f,x, y, CurrentWorld.Map);
        float targetCameraX = CurrentWorld.Player.GetVisualX() - 3.5f; // 3.5 centers the 16px player better in an 8-tile view
        float targetCameraY = CurrentWorld.Player.GetVisualY() - 3.5f;

        float cameraLerpSpeed = 0.2f; // Camera should be slightly "lazier" than player
        CameraX += (targetCameraX - CameraX) * cameraLerpSpeed;
        CameraY += (targetCameraY - CameraY) * cameraLerpSpeed;
        CurrentWorld.Update();
        
        var attackVector = InputService.GetAttackVector();
        if (attackVector.HasValue)
        {
            var click = attackVector;
            if (click == null) return;

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
                
            };
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

            var paint = new SKPaint { FilterQuality = SKFilterQuality.None };
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

            DrawPlayer(world, _viewLeft, _viewTop, canvas);

            DrawEnemies(world, startX, endX, startY, endY, canvas);
            
             DrawEvents(world, startX, endX, startY, endY, canvas);
           
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

            var animation = Graphics.TileCoordinates.Character.ExplosionAnimation;
            var frame = (DateTime.Now - current.StartTime).TotalSeconds / 1.0 * animation.Length; // Assuming 1 second duration for full animation
            int frameIndex = Math.Min(animation.Length - 1, (int)frame);
            var eCoords = animation[frameIndex];
            var eSrc = new SKRect(eCoords.x, eCoords.y, eCoords.x + 16, eCoords.y + 16);
            float eLeft = (current.X * 16);
            float eTop = (current.Y * 16);
            var eDest = new SKRect(eLeft, eTop, eLeft + 16.0f, eTop + 16.0f);
                
            var directionX = 1;
            DrawSprite(canvas, eSrc, eDest, 1,0,directionX);
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
                int[] baseCoords = Graphics.TileCoordinates.GetTileBaseCoordinates(tile.type);
                int[] decorCoords = Graphics.TileCoordinates.GetDecorationTileCoordinates(tile.type);

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
            var eCoords = enemy.GetActiveFrame() == -1 ? Graphics.TileCoordinates.Enemy.Idle : Graphics.TileCoordinates.Enemy.RunAnimation
                [enemy.GetActiveFrame()];
            var eSrc = new SKRect(eCoords.x, eCoords.y, eCoords.x + 16, eCoords.y + 16);
            float eLeft = (enemy.GetVisualX() * 16);
            float eTop = (enemy.GetVisualY() * 16);
            var eDest = new SKRect(eLeft, eTop, eLeft + 16.0f, eTop + 16.0f);
                
            var directionX = enemy.GetDirectionX();
            DrawSprite(canvas, eSrc, eDest, 1,0,directionX);
        }
    }

    private void DrawPlayer(World world, float viewLeft, float viewTop, SKCanvas canvas)
    {
        var pCoords = world.Player.GetActiveFrame() == -1 ? Graphics.TileCoordinates.Player.Idle : Graphics.TileCoordinates.Player.RunAnimation
            [world.Player.GetActiveFrame()];
        
        var pSrc = new SKRect(pCoords.x, pCoords.y, pCoords.x + 16, pCoords.y + 16);
        float left = viewLeft + 4 * 16;
        float f = viewLeft + 16 * 4;
        var pDest = new SKRect(left, viewTop + 16 * 4, f + 16.0f, viewTop + 16 * 4 + 16.0f);
        DrawSprite(canvas, pSrc, pDest, 1,0,world.Player.GetDirectionX());
    }

    private void DrawSprite(SKCanvas canvas, SKRect sourceRect, SKRect destination, float scale = 1.0f, float rotationDegrees = 0, float flipX = 1)
    {
        canvas.Save();

        float width = sourceRect.Width;
        float height = sourceRect.Height;
        canvas.Translate(destination.Left + width / 2, destination.Top + height / 2);

        if (rotationDegrees != 0)
        {
            canvas.RotateDegrees(rotationDegrees);
        }

        canvas.Scale(flipX, 1);

        SKRect destRect = new SKRect(-width / 2, -height / 2, width / 2, height / 2);
        canvas.DrawBitmap(_tileSheet, sourceRect, destRect);

        canvas.Restore();
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
    
}