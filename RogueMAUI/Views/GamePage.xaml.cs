using RogueMAUI.Services;
using SkiaSharp;
using SKPaintSurfaceEventArgs = SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs;
using RogueMAUI.ViewModels;

namespace RogueMAUI.Views;

public partial class GamePage : ContentPage
{
    private readonly GameViewModel _viewModel;
    private SKBitmap _tileSheet;
    private bool _isGameRunning;

    public GamePage(IInputService inputService)
    {
        InitializeComponent();

        _viewModel = new GameViewModel(inputService);
        BindingContext = _viewModel;

        LoadAssets();
        _viewModel.Initialize();
        _isGameRunning = true;
        Task.Run(GameLoop);
    }
    private async Task GameLoop()
    {
        while (_isGameRunning)
        {
            _viewModel.Update();
        
            MainThread.BeginInvokeOnMainThread(() => {
                GameCanvas.InvalidateSurface(); 
            });

            await Task.Delay(16);
        }
    }
    
    private void LoadAssets()
    {
        using var stream = FileSystem.OpenAppPackageFileAsync("tilemap.png").Result;
        _tileSheet = SKBitmap.Decode(stream);
    }

    private void OnCanvasPaint(object sender, SKPaintSurfaceEventArgs e)
    {
        if (_viewModel.CurrentWorld == null) return;

        SKImageInfo info = e.Info;
        SKSurface surface = e.Surface;
        SKCanvas canvas = surface.Canvas;

        canvas.Clear(SKColors.Black);

        float scale = Math.Min((float)info.Width / 128, (float)info.Height / 128);
        float tx = (info.Width - 128 * scale) / 2;
        float ty = (info.Height - 128 * scale) / 2;

        canvas.Translate(tx, ty);
        canvas.Scale(scale);

        var paint = new SKPaint { FilterQuality = SKFilterQuality.None };
        var world = _viewModel.CurrentWorld;
        int startX = world.Player.GetX() - 4;
        int startY = world.Player.GetY() - 4;

        for (int x = startX; x < startX + 8; x++)
        {
            for (int y = startY; y < startY + 8; y++)
            {
                int clampedX = Math.Max(0, Math.Min(world.Map.Count - 1, x));
                int clampedY = Math.Max(0, Math.Min(world.Map[clampedX].Count - 1, y));
                int[] baseCoords = Graphics.TileCoordinates.GetTileBaseCoordinates(world.Map[clampedX][clampedY].type);
                var src = new SKRect(baseCoords[0], baseCoords[1], baseCoords[0] + 16, baseCoords[1] + 16);
                var dest = new SKRect((clampedX - startX) * 16, (clampedY - startY) * 16, (clampedX - startX + 1) * 16, (clampedY - startY + 1) * 16);
                canvas.DrawBitmap(_tileSheet, src, dest, paint);
            }
        }

        var playerCoords = Graphics.TileCoordinates.Player.Idle;
        canvas.DrawBitmap(_tileSheet,
            new SKRect(playerCoords.x, playerCoords.y, playerCoords.x + 16, playerCoords.y + 16),
            new SKRect(48, 48, 64, 64), paint);
    }
}