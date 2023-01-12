using Plugin.Firebase.Auth;
using static TinderButForBartering.Data;

namespace TinderButForBartering;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new NavigationPage(new MainPage());

        _ = GetOwnProducts(); // what should be done if this operation fails?
    }

	protected override async void OnStart()
	{
		base.OnStart();

		if (CrossFirebaseAuth.Current.CurrentUser == null)
		{
			await MainPage.Navigation.PushModalAsync(new LoginPage());
		}
	}
}
