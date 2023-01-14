using Plugin.Firebase.Auth;

namespace TinderButForBartering;

public class Auth
{
    public static async Task<bool> SignInWithGoogleAsync()
    {
        try
        {
            IFirebaseUser user = await CrossFirebaseAuth.Current.SignInWithGoogleAsync();
            await App.Current.MainPage.DisplayAlert("User signed in", $"{user.DisplayName}, {user.Uid}, {user.Email}, {user.ToString()}, {user.IsEmailVerified}", "OK");
            await Data.OnLogin(user);
            return true;
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            return false;
        }
    }

    public static async Task<bool> SignInWithFacebookAsync()
    {
        try
        {
            IFirebaseUser user = await CrossFirebaseAuth.Current.SignInWithFacebookAsync();
            await App.Current.MainPage.DisplayAlert("User signed in", $"{user.DisplayName}, {user.Uid}, {user.Email}, {user.ToString()}, {user.IsEmailVerified}", "OK");
            await Data.OnLogin(user);
            return true;
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            return false;
        }
    }

    public static async Task<bool> SignUpWithEmailAndPasswordAsync(string name, string email, string password)
    {
        try
        {
            await CrossFirebaseAuth.Current.CreateUserAsync(email, password);
            IFirebaseUser user = CrossFirebaseAuth.Current.CurrentUser;
            await user.UpdateProfileAsync(displayName: name);
            await App.Current.MainPage.DisplayAlert("User signed in", $"{user.DisplayName}, {user.Uid}, {user.Email}, {user.ToString()}, {user.IsEmailVerified}", "OK");
            await Data.OnLogin(user);
            return true;
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            return false;
        }
    }

    public static async Task<bool> SignInWithEmailAndPasswordAsync(string email, string password)
    {
        try
        {
            IFirebaseUser user = await CrossFirebaseAuth.Current.SignInWithEmailAndPasswordAsync(email, password, createsUserAutomatically: false);
            await App.Current.MainPage.DisplayAlert("User signed in", $"{user.DisplayName}, {user.Uid}, {user.Email}, {user.ToString()}, {user.IsEmailVerified}", "OK");
            await Data.OnLogin(user);
            return true;
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            return false;
        }
    }

#nullable enable
    public static async Task<bool> PasswordResetAsync(string? email)
#nullable disable
    {
        try
        {
            await CrossFirebaseAuth.Current.SendPasswordResetEmailAsync(email);
            return true;
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            return false;
        }
    }

    public static async Task<bool> SignOutAsync()
    {
        try
        {
            await CrossFirebaseAuth.Current.SignOutAsync();
            Data.DeleteLocalData();
            return true;
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            return false;
        }
    }

    public static async Task<bool> DeleteAccountAsync()
    {
        try
        {
            await CrossFirebaseAuth.Current.CurrentUser.DeleteAsync();
            Data.DeleteLocalData();
            return true;
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            return false;
        }
    }
}
