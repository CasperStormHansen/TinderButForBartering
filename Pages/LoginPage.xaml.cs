using Plugin.Firebase.Auth;
using Plugin.Firebase.Common;

namespace TinderButForBartering;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void LoginButton_Clicked(object sender, EventArgs e)
    {
        LoginButton.IsEnabled = false;
        // busy

        await SignInWithEmailAndPasswordAsync();

        // not busy

        if (false)
        {
            await App.Current.MainPage.DisplayAlert("Error", "Something went wrong logging you in. Please try again.", "OK");
            LoginButton.IsEnabled = true;
            // not busy
        }
        else
        {
            await Navigation.PopModalAsync();
        }
    }

    private async Task SignInWithEmailAndPasswordAsync()
    {
        try
        {
            var auth = CrossFirebaseAuth.Current;
            var email = "casperstormhansen2@gmail.com";
            var password = "Aa12345_";
            var user = await auth.SignInWithEmailAndPasswordAsync(email, password);
            Console.WriteLine($"User signed in: {user.Email}");
        }
        catch (FirebaseAuthException ex)
        {
            Console.WriteLine($"Error signing in: {ex.Message}");
        }
    }
}