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

        (bool success, string errorMessage) = await Auth.SignUpWithEmailAndPasswordAsync(NameEntry.Text.Trim(), SignupEmailEntry.Text.Trim(), SignupPasswordEntry.Text.Trim());

        BusyIndicator.IsVisible = false;

        if (success)
        {
            await Navigation.PopModalAsync();
            Data.MainPage.OnAnyAppearance();
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("Fejl", errorMessage, "OK");
            LoginEmailButton.IsEnabled = true;
            BusyIndicator.IsVisible = false;
        }
    }

    private async void LoginEmailButton_Clicked(object sender, EventArgs e)
    {
        LoginEmailButton.IsEnabled = false;
        BusyIndicator.IsVisible = true;

        (bool success, string errorMessage) = await Auth.SignInWithEmailAndPasswordAsync(LoginEmailEntry.Text.Trim(), LoginPasswordEntry.Text.Trim());

        BusyIndicator.IsVisible = false;

        if (success)
        {
            await Navigation.PopModalAsync();
            Data.MainPage.OnAnyAppearance();
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("Fejl", errorMessage, "OK");
            LoginEmailButton.IsEnabled = true;
            BusyIndicator.IsVisible = false;
        }
    }

    private async void ForgottenPasswordButton_Clicked(object sender, EventArgs e)
    {
        ForgottenPasswordButton.IsEnabled = false;
        BusyIndicator.IsVisible = true;

        (bool success, string errorMessage) = await Auth.PasswordResetAsync(LoginEmailEntry.Text.Trim());

        if (success)
        {
            await App.Current.MainPage.DisplayAlert("Email sendt", "Følg venligst linket i den sendte email.", "OK");
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("Fejl", errorMessage, "OK");
        }
        ForgottenPasswordButton.IsEnabled = true;
        BusyIndicator.IsVisible = false;
    }

    private async void LoginFacebookButton_Clicked(object sender, EventArgs e)
    {
        LoginFacebookButton.IsEnabled = false;
        BusyIndicator.IsVisible = true;

        (bool success, string errorMessage) = await Auth.SignInWithFacebookAsync();

        BusyIndicator.IsVisible = false;

        if (success)
        {
            await Navigation.PopModalAsync();
            Data.MainPage.OnAnyAppearance();
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("Fejl", errorMessage, "OK");
            LoginFacebookButton.IsEnabled = true;
            BusyIndicator.IsVisible = false;
        }
    }

    private async void LoginGoogleButton_Clicked(object sender, EventArgs e)
    {
        LoginGoogleButton.IsEnabled = false;
        BusyIndicator.IsVisible = true;

        (bool success, string errorMessage) = await Auth.SignInWithGoogleAsync();

        BusyIndicator.IsVisible = false;

        if (success)
        {
            await Navigation.PopModalAsync();
            Data.MainPage.OnAnyAppearance();
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("Fejl", errorMessage, "OK");
            LoginGoogleButton.IsEnabled = true;
            BusyIndicator.IsVisible = false;
        }
    }
}