namespace RogueMAUI;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}


	
	private async void OnStartGameClicked(object sender, EventArgs e)
	{
		// Navigate using the route name you registered
		await Shell.Current.GoToAsync("GamePage");
	}
}
