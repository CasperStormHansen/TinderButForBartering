using Plugin.Firebase.Auth;

namespace TinderButForBartering;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new NavigationPage(new MainPage());
    }

	protected override async void OnStart()
	{
		base.OnStart();

		if (CrossFirebaseAuth.Current.CurrentUser == null)
		{
			await MainPage.Navigation.PushModalAsync(new LoginPage());
		}
		else
		{
			await Data.OnLogin(CrossFirebaseAuth.Current.CurrentUser); // add error handling
		}
	}
}
