using Plugin.Firebase.Auth;

namespace TinderButForBartering;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (CrossFirebaseAuth.Current.CurrentUser != null)
        {
            //TestLabel.Text = CrossFirebaseAuth.Current.CurrentUser.ToString() + CrossFirebaseAuth.Current.CurrentUser.DisplayName; 
            //UserPicture.Source = CrossFirebaseAuth.Current.CurrentUser.PhotoUrl;
        }
        else
        {
            //UserPicture.Source = null;
        }
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (CrossFirebaseAuth.Current.CurrentUser != null)
        {
            TestLabel.Text = CrossFirebaseAuth.Current.CurrentUser.ToString() + CrossFirebaseAuth.Current.CurrentUser.DisplayName;
            UserPicture.Source = CrossFirebaseAuth.Current.CurrentUser.PhotoUrl;
        }
        else
        {
            UserPicture.Source = null;
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
        LogoutButton.IsEnabled = false;
        BusyIndicator.IsVisible = true;

        bool succes = await Auth.SignOutAsync();

        if (succes)
        {
            await Navigation.PushModalAsync(new LoginPage());
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("Error", "Something went wrong. Please try again.", "OK");
        }
        LogoutButton.IsEnabled = true;
        BusyIndicator.IsVisible = false;
    }

    private async void OnDeleteAccountButton_Clicked(object sender, EventArgs e)
    {
        DeleteAccountButton.IsEnabled = false;
        BusyIndicator.IsVisible = true;

        bool succes = await Auth.DeleteAccountAsync();

        if (succes)
        {
            await Navigation.PushModalAsync(new LoginPage());
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("Error", "Something went wrong. Please try again.", "OK");
        }
        DeleteAccountButton.IsEnabled = true;
        BusyIndicator.IsVisible = false;
    }
}

