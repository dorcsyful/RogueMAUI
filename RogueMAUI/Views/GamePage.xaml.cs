using System;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using RogueCore.Helpers;
using RogueCore.Models;
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

            await Task.Delay(16); // Slow down temporarily for debugging
        }
    }
    
    private void LoadAssets()
    {
        using var stream = FileSystem.OpenAppPackageFileAsync("tilemap.png").Result;
        _tileSheet = SKBitmap.Decode(stream);
    }

    private void OnCanvasPaint(object sender, SKPaintSurfaceEventArgs e)
    {
        try
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear(SKColors.Black);

            var paint = new SKPaint { FilterQuality = SKFilterQuality.None };
            var world = _viewModel.CurrentWorld;

            // (128x128 pixels = 8x8 tiles)
            float viewWidth = 128.0f;
            float viewHeight = 128.0f;


            float viewCenterX = world.Player.GetVisualX() * 16.0f;
            float viewCenterY = world.Player.GetVisualY() * 16.0f;

            // Calculate top-left corner of view window
            float viewLeft = viewCenterX - viewWidth / 2.0f;
            float viewTop = viewCenterY - viewHeight / 2.0f;

            // Scale to fit canvas while maintaining square aspect
            float canvasWidth = info.Width;
            float canvasHeight = info.Height;
            float scale = Math.Min(canvasWidth / viewWidth, canvasHeight / viewHeight);

            // Calculate offsets to center the scaled view on canvas
            float scaledViewSize = viewWidth * scale;
            float tx = (canvasWidth - scaledViewSize) / 2.0f;
            float ty = (canvasHeight - scaledViewSize) / 2.0f;

            // Apply transformations: translate to center, scale up, then translate to view position
            canvas.Translate(tx, ty);
            canvas.Scale(scale);
            canvas.Translate(-viewLeft, -viewTop);

            // Calculate which tiles are visible in the view window
            int startX = Math.Max(0, (int)Math.Floor(viewLeft / 16.0f));
            int startY = Math.Max(0, (int)Math.Floor(viewTop / 16.0f));
            int endX = Math.Min(100, (int)Math.Ceiling((viewLeft + viewWidth) / 16.0f));
            int endY = Math.Min(100, (int)Math.Ceiling((viewTop + viewHeight) / 16.0f));

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
            
            var pCoords = Graphics.TileCoordinates.Player.Idle;
        
            var pSrc = new SKRect(pCoords.x, pCoords.y, pCoords.x + 16, pCoords.y + 16);
            float left = viewLeft + 4 * 16;
            float f = viewLeft + 16 * 4;
            var pDest = new SKRect(left, viewTop + 16 * 4, f + 16.0f, viewTop + 16 * 4 + 16.0f);
        
            canvas.DrawBitmap(_tileSheet, pSrc, pDest, paint);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Paint error: {ex.Message}");
        }
    

    }
}