using Plugin.Firebase.Auth;
using System.Collections.Specialized;

namespace TinderButForBartering;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
        Data.MainPage = this;
        RefreshView.Command = new Command(RefreshMethod);
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        OnAnyAppearance();
    }

    public async void OnAnyAppearance()
    {
        if (CrossFirebaseAuth.Current.CurrentUser != null)
        {
            if (MyGoodsButton.IsEnabled)
            {
                ShowNextProduct();
            }
            else
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
        }

        // The following two lines should not be necessary, but are due to a bug
        RefreshView.HeightRequest = Application.Current.MainPage.Height;
        RefreshView.WidthRequest = Application.Current.MainPage.Width;
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

    private void ShowNextProduct() // Optimization possible: call this method when and only when the front of the queue changes
    {
        if (Data.SwipingProducts.TryPeek(out Product product))
        {
            Application.Current.MainPage.Dispatcher.Dispatch(() =>
            {
                SwipingPicture.Source = product.Url;
                DetailsButton.IsEnabled = true;
                YesButton.IsEnabled = true;
                NoButton.IsEnabled = true;
                WillPayButton.IsEnabled = true;                
            });

            Data.SwipingProducts.CollectionChanged -= OnSwipingProductsMadeNonEmpty;
        }
        else
        {
            Application.Current.MainPage.Dispatcher.Dispatch(() =>
            {
                SwipingPicture.Source = "nomoreswipingproducts.jpg";
                DetailsButton.IsEnabled = false;
                YesButton.IsEnabled = false;
                NoButton.IsEnabled = false;
                WillPayButton.IsEnabled = false;
            });

            Data.SwipingProducts.CollectionChanged += OnSwipingProductsMadeNonEmpty;
        }
    }

    private void OnSwipingProductsMadeNonEmpty(object sender, NotifyCollectionChangedEventArgs e)
    {
        ShowNextProduct();
    }

    private async void OnDetailsButton_Clicked(object sender, EventArgs e)
    {
        Data.SwipingProducts.TryPeek(out Product product);
        await Navigation.PushAsync(new ForeignProductsDetailsPage(product, true));
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

    private async void RefreshMethod(object obj)
    {
        RefreshView.IsRefreshing = true;
        await Data.OnRefreshMainpage();
        ShowNextProduct();
        RefreshView.IsRefreshing = false;
    }
}