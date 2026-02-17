using RogueMAUI.Views;

namespace RogueMAUI;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		// Register the route name for the GamePage
		Routing.RegisterRoute("GamePage", typeof(GamePage));
	}
}
