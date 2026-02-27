using System;
using System.Runtime.CompilerServices;
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
    private bool _isGameRunning;

    public GamePage(IInputService inputService)
    {
        InitializeComponent();

        _viewModel = new GameViewModel(inputService);
        BindingContext = _viewModel;

        //LoadAssets();
        _viewModel.Initialize();
        _isGameRunning = true;
        Task.Run(GameLoop);
    }
    private async Task GameLoop()
    {
        while (_isGameRunning)
        {
            if (_viewModel.QuitRequested)
            {
                _viewModel.QuitRequested = false;
                await Shell.Current.GoToAsync("MainPage");
            }
            _viewModel.Update();
        
            MainThread.BeginInvokeOnMainThread(() => {
                GameCanvas.InvalidateSurface(); 
            });

            await Task.Delay(16); 
        }
    }
    


    private void OnCanvasPaint(object sender, SKPaintSurfaceEventArgs e)
    {
        _viewModel.Draw(e);
    }


}