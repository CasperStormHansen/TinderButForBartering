namespace TinderButForBartering;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void SignupEmailButton_Clicked(object sender, EventArgs e)
    {
        LoginEmailButton.IsEnabled = false;
        BusyIndicator.IsVisible = true;

        bool succes = await Auth.SignUpWithEmailAndPasswordAsync(NameEntry.Text.Trim(), SignupEmailEntry.Text.Trim(), SignupPasswordEntry.Text.Trim());

        BusyIndicator.IsVisible = false;

        if (succes)
        {
            await Navigation.PopModalAsync();
            Data.MainPage.OnAnyAppearance();
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("Error", "Something went wrong signing you up. Please try again.", "OK");
            LoginEmailButton.IsEnabled = true;
            BusyIndicator.IsVisible = false;
        }
    }

    private async void LoginEmailButton_Clicked(object sender, EventArgs e)
    {
        LoginEmailButton.IsEnabled = false;
        BusyIndicator.IsVisible = true;

        bool succes = await Auth.SignInWithEmailAndPasswordAsync(LoginEmailEntry.Text.Trim(), LoginPasswordEntry.Text.Trim());

        BusyIndicator.IsVisible = false;

        if (succes)
        {
            await Navigation.PopModalAsync();
            Data.MainPage.OnAnyAppearance();
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("Error", "Something went wrong logging you in. Please try again.", "OK");
            LoginEmailButton.IsEnabled = true;
            BusyIndicator.IsVisible = false;
        }
    }

    private async void ForgottenPasswordButton_Clicked(object sender, EventArgs e)
    {
        ForgottenPasswordButton.IsEnabled = false;
        BusyIndicator.IsVisible = true;

        bool succes = await Auth.PasswordResetAsync(LoginEmailEntry.Text.Trim());

        if (succes)
        {
            await App.Current.MainPage.DisplayAlert("Email sendt", "Følg venligst linket i den sendte email.", "OK");
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("Error", "Something went wrong.", "OK");
        }
        ForgottenPasswordButton.IsEnabled = true;
        BusyIndicator.IsVisible = false;
    }

    private async void LoginFacebookButton_Clicked(object sender, EventArgs e)
    {
        LoginFacebookButton.IsEnabled = false;
        BusyIndicator.IsVisible = true;

        bool succes = await Auth.SignInWithFacebookAsync();

        BusyIndicator.IsVisible = false;

        if (succes)
        {
            await Navigation.PopModalAsync();
            Data.MainPage.OnAnyAppearance();
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

        bool succes = await Auth.SignInWithGoogleAsync();

        BusyIndicator.IsVisible = false;

        if (succes)
        {
            await Navigation.PopModalAsync();
            Data.MainPage.OnAnyAppearance();
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("Error", "Something went wrong logging you in. Please try again.", "OK");
            LoginGoogleButton.IsEnabled = true;
            BusyIndicator.IsVisible = false;
        }
    }
}