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
                (success, string errorMessage) = await Data.OnLogin(CrossFirebaseAuth.Current.CurrentUser);
                if (success)
                {
                    ShowNextProduct();
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Der kunne ikke opnås kontakt til serveren", errorMessage, "Prøv igen");
                }
            }
            BusyIndicator.IsVisible = false;

            MyGoodsButton.IsEnabled = true;
            MyWishesButton.IsEnabled = true;
            MyMatchesButton.IsEnabled = true;
        }

        if (CrossFirebaseAuth.Current.CurrentUser != null && MyGoodsButton.IsEnabled)
        {
            ShowNextProduct();
        }
    }

    private async void OnHamburgerIcon_Clicked(object sender, EventArgs e)
    {
        var result = await App.Current.MainPage.DisplayActionSheet("Menu", "Tilbage", " ", "Log ud", "Slet konto");

        if (result == "Log ud")
        {
            MyGoodsButton.IsEnabled = false;
            MyWishesButton.IsEnabled = false;
            MyMatchesButton.IsEnabled = false;
;
            BusyIndicator.IsVisible = true;

            (bool success, string errorMessage) = await Auth.SignOutAsync();

            if (success)
            {
                Data.Logout();
                await Navigation.PushModalAsync(new LoginPage());
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Fejl", errorMessage, "OK");
            }

            BusyIndicator.IsVisible = false;
        }
        else if (result == "Slet konto")
        {
            MyGoodsButton.IsEnabled = false;
            MyWishesButton.IsEnabled = false;
            MyMatchesButton.IsEnabled = false;

            BusyIndicator.IsVisible = true;

            (bool success, string errorMessage) = await Auth.DeleteAccountAsync();

            if (success)
            {
                Data.Logout();
                await Navigation.PushModalAsync(new LoginPage());
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Fejl", errorMessage, "OK");
            }

            BusyIndicator.IsVisible = false;
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

    private void ShowNextProduct()
    {
        if (Data.SwipingProducts.TryPeek(out Product product))
        {
            SwipingPicture.Source = product.Url;
            DetailsButton.IsEnabled = true;
            YesButton.IsEnabled = true;
            NoButton.IsEnabled = true;
            WillPayButton.IsEnabled = true;
        }
        else
        {
            SwipingPicture.Source = "nomoreswipingproducts.jpg";
            DetailsButton.IsEnabled = false;
            YesButton.IsEnabled = false;
            NoButton.IsEnabled = false;
            WillPayButton.IsEnabled = false;
            // make this function run again on changes to SwipingProducts
        }
    }

    private async void OnDetailsButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ForeignProductsDetailsPage(Data.SwipingProducts.Peek(), true));
    }

    private async void OnNoButton_Clicked(object sender, EventArgs e)
    {
        await Data.OnSwipe("NoToProduct");
        ShowNextProduct();
    }

    private async void OnYesButton_Clicked(object sender, EventArgs e)
    {
        await Data.OnSwipe("YesToProduct");
        ShowNextProduct();
    }

    private async void OnWillPayButton_Clicked(object sender, EventArgs e)
    {
        await Data.OnSwipe("WillPayForProduct");
        ShowNextProduct();
    }
}