using Plugin.Firebase.Auth;
using Plugin.Firebase.Common;

namespace TinderButForBartering;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void LoginEmailButton_Clicked(object sender, EventArgs e)
    {
        LoginEmailButton.IsEnabled = false;
        BusyIndicator.IsVisible = true;

        bool succes = await SignInWithEmailAndPasswordAsync();

        BusyIndicator.IsVisible = false;

        if (succes)
        {
            await Navigation.PopModalAsync();
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("Error", "Something went wrong logging you in. Please try again.", "OK");
            LoginEmailButton.IsEnabled = true;
            BusyIndicator.IsVisible = false;
        }
    }

    private async void LoginFacebookButton_Clicked(object sender, EventArgs e)
    {
        LoginFacebookButton.IsEnabled = false;
        BusyIndicator.IsVisible = true;

        bool succes = await SignInWithFacebookAsync();

        BusyIndicator.IsVisible = false;

        if (succes)
        {
            await Navigation.PopModalAsync();
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("Error", "Something went wrong logging you in. Please try again.", "OK");
            LoginFacebookButton.IsEnabled = true;
            BusyIndicator.IsVisible = false;
        }
    }

    private async void LoginGoogleButton_Clicked(object sender, EventArgs e)
    {
        LoginGoogleButton.IsEnabled = false;
        BusyIndicator.IsVisible = true;

        bool succes = await SignInWithGoogleAsync();

        BusyIndicator.IsVisible = false;

        if (succes)
        {
            await Navigation.PopModalAsync();
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("Error", "Something went wrong logging you in. Please try again.", "OK");
            LoginGoogleButton.IsEnabled = true;
            BusyIndicator.IsVisible = false;
        }
    }

    private static async Task<bool> SignInWithEmailAndPasswordAsync()
    {
        try
        {
            string email = "casperstormhansen2@gmail.com";
            string password = "Aa12345_";
            IFirebaseUser user = await CrossFirebaseAuth.Current.SignInWithEmailAndPasswordAsync(email, password);
            await App.Current.MainPage.DisplayAlert("User signed in", $"{user.DisplayName}, {user.Uid}, {user.Email}, {user.ToString()}, {user.IsEmailVerified}", "OK");
            return true;
        }
        catch (FirebaseAuthException ex)
        {
            await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            return false;
        }
    }

    private static async Task<bool> SignInWithGoogleAsync()
    {
        try
        {
            IFirebaseUser user = await CrossFirebaseAuth.Current.SignInWithGoogleAsync();
            await App.Current.MainPage.DisplayAlert("User signed in", $"{user.DisplayName}, {user.Uid}, {user.Email}, {user.ToString()}, {user.IsEmailVerified}", "OK");
            return true;
        }
        catch (FirebaseAuthException ex)
        {
            await App.Current.MainPage.DisplayAlert("Error", ex.ToString() + ex.Reason, "OK");
            if (ex.InnerException != null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "inner exception is not null", "OK");
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "inner exception is null", "OK");
            }
            return false;
        }
    }

    private static async Task<bool> SignInWithFacebookAsync()
    {
        try
        {
            IFirebaseUser user = await CrossFirebaseAuth.Current.SignInWithFacebookAsync();
            await App.Current.MainPage.DisplayAlert("User signed in", $"{user.DisplayName}, {user.Uid}, {user.Email}, {user.ToString()}, {user.IsEmailVerified}", "OK");
            return true;
        }
        catch (FirebaseAuthException ex)
        {
            await App.Current.MainPage.DisplayAlert("Error", ex.ToString() + ex.Reason, "OK");
            return false;
        }
    }
}