using Microsoft.Maui.Controls;
using Plugin.Firebase.Auth;

namespace TinderButForBartering;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
        Data.MainPage = this;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        OnAnyAppearance();
    }

    public async void OnAnyAppearance()
    {
        if (CrossFirebaseAuth.Current.CurrentUser != null && !MyGoodsButton.IsEnabled)
        {
            BusyIndicator.IsVisible = true;
            bool success = false;
            while (!success)
            {
                (success, string errorMesssage) = await Data.OnLogin(CrossFirebaseAuth.Current.CurrentUser);
                if (!success)
                {
                    await App.Current.MainPage.DisplayAlert("Der kunne ikke opnås kontakt til serveren", errorMesssage, "Prøv igen");
                }
            }
            BusyIndicator.IsVisible = false;

            MyGoodsButton.IsEnabled = true;
            MyWishesButton.IsEnabled = true;
            MyMatchesButton.IsEnabled = true;
        }
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
        MyGoodsButton.IsEnabled = false;
        MyWishesButton.IsEnabled = false;
        MyMatchesButton.IsEnabled = false;

        LogoutButton.IsEnabled = false;
        BusyIndicator.IsVisible = true;

        bool success = await Auth.SignOutAsync();

        if (success)
        {
            Data.DeleteLocalData();
            await Navigation.PushModalAsync(new LoginPage());
        }

        LogoutButton.IsEnabled = true;
        BusyIndicator.IsVisible = false;
    }

    private async void OnDeleteAccountButton_Clicked(object sender, EventArgs e)
    {
        MyGoodsButton.IsEnabled = false;
        MyWishesButton.IsEnabled = false;
        MyMatchesButton.IsEnabled = false;
        
        DeleteAccountButton.IsEnabled = false;
        BusyIndicator.IsVisible = true;

        bool success = await Auth.DeleteAccountAsync();

        if (success)
        {
            Data.DeleteLocalData();
            await Navigation.PushModalAsync(new LoginPage());
        }
       
        DeleteAccountButton.IsEnabled = true;
        BusyIndicator.IsVisible = false;
    }
}