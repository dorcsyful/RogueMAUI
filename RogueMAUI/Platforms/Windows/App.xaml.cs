using Microsoft.UI.Xaml;
using RogueMAUI.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RogueMAUI.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
	/// <summary>
	/// Initializes the singleton application object.  This is the first line of authored code
	/// executed, and as such is the logical equivalent of main() or WinMain().
	/// </summary>
	public App()
	{
		this.InitializeComponent();
		
	}
	protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
	{
		base.OnLaunched(args);

		var mauiWindow = Microsoft.Maui.Controls.Application.Current.Windows[0];
    
		var nativeWindow = mauiWindow.Handler.PlatformView as Microsoft.UI.Xaml.Window;

		var inputService = IPlatformApplication.Current.Services.GetService<IInputService>() as WindowsInputService;

		if (nativeWindow != null && inputService != null)
		{
			inputService.Initialize(nativeWindow);
		}
	}
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}

