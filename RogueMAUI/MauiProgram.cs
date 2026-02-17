using Microsoft.Extensions.Logging;
using RogueMAUI.Services;
using RogueMAUI.ViewModels;
using RogueMAUI.Views;
using SkiaSharp.Views.Maui.Controls.Hosting; 
namespace RogueMAUI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseSkiaSharp()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
		#if WINDOWS
				builder.Services.AddSingleton<IInputService, RogueMAUI.WindowsInputService>();
		#elif ANDROID
		        builder.Services.AddSingleton<IInputService, RogueMAUI.AndroidInputService>();
		#endif
		builder.Services.AddSingleton<GameViewModel>();
		builder.Services.AddSingleton<GamePage>();
#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
