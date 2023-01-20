using Plugin.Firebase.Auth;

namespace TinderButForBartering;

public class Auth
{
    public static async Task<(bool, string)> SignInWithGoogleAsync()
    {
        try
        {
            IFirebaseUser user = await CrossFirebaseAuth.Current.SignInWithGoogleAsync();
            return (true, "");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public static async Task<(bool, string)> SignInWithFacebookAsync()
    {
        try
        {
            IFirebaseUser user = await CrossFirebaseAuth.Current.SignInWithFacebookAsync();
            return (true, "");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public static async Task<(bool, string)> SignUpWithEmailAndPasswordAsync(string name, string email, string password)
    {
        try
        {
            await CrossFirebaseAuth.Current.CreateUserAsync(email, password);
            IFirebaseUser user = CrossFirebaseAuth.Current.CurrentUser;
            await user.UpdateProfileAsync(displayName: name);
            return (true, "");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public static async Task<(bool, string)> SignInWithEmailAndPasswordAsync(string email, string password)
    {
        try
        {
            IFirebaseUser user = await CrossFirebaseAuth.Current.SignInWithEmailAndPasswordAsync(email, password, createsUserAutomatically: false);
            return (true, "");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

#nullable enable
    public static async Task<(bool, string)> PasswordResetAsync(string? email)
#nullable disable
    {
        try
        {
            await CrossFirebaseAuth.Current.SendPasswordResetEmailAsync(email);
            return (true, "");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public static async Task<(bool, string)> SignOutAsync()
    {
        try
        {
            await CrossFirebaseAuth.Current.SignOutAsync();
            return (true, "");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public static async Task<(bool, string)> DeleteAccountAsync()
    {
        try
        {
            await CrossFirebaseAuth.Current.CurrentUser.DeleteAsync();
            return (true, "");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
}
