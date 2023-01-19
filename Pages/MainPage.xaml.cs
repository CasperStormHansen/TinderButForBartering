namespace TinderButForBartering;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

    private async void OnMyGoodsButton_Clicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new MyGoodsPage());
	}

    private async void OnMyWishesButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MyWishesPage());
    }
    private async void OnMyMatchesButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MyMatchesPage());
    }

    private async void OnLogoutButton_Clicked(object sender, EventArgs e)
    {
        LogoutButton.IsEnabled = false;
        BusyIndicator.IsVisible = true;

        bool success = await Auth.SignOutAsync();

        if (success)
        {
            await Navigation.PushModalAsync(new LoginPage());
        }

        LogoutButton.IsEnabled = true;
        BusyIndicator.IsVisible = false;
    }

    private async void OnDeleteAccountButton_Clicked(object sender, EventArgs e)
    {
        DeleteAccountButton.IsEnabled = false;
        BusyIndicator.IsVisible = true;

        bool success = await Auth.DeleteAccountAsync();

        if (success)
        {
            await Navigation.PushModalAsync(new LoginPage());
        }
       
        DeleteAccountButton.IsEnabled = true;
        BusyIndicator.IsVisible = false;
    }
}