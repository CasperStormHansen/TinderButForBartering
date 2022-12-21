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
}
