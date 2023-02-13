using Microsoft.AspNetCore.SignalR.Client;

namespace TinderButForBartering;

public class Backend
{
#if ANDROID
    static readonly string BaseUrl = "http://10.0.2.2:5045/";
#else
    static readonly string BaseUrl = "https://localhost:7239/";
#endif

    static readonly string ComHubUrl = BaseUrl + "comhub";

    private static HubConnection ComHubConnection { get; set; }

    /// <summary>
    /// Attempts to create SignalR connection with backend. Only to be called from OnLogin, which
    /// handles registration and errors.
    /// </summary>
    private static async Task Connect()
    {
        ComHubConnection = new HubConnectionBuilder()
            .WithUrl(ComHubUrl)
            .Build();

        ComHubConnection.On<Message>("ReceiveMessage", Data.ReceiveMessage);
        ComHubConnection.On<Match>("ReceiveMatch", Data.ReceiveMatch);
        ComHubConnection.On<Product, int>("AddForeignProductToMatch", Data.AddForeignProductToMatch);
        ComHubConnection.On<int, int>("AddOwnProductToMatch", Data.AddOwnProductToMatch);
        ComHubConnection.On<Product, int>("UpdateForeignProductInMatch", Data.UpdateForeignProductInMatch);

        await ComHubConnection.StartAsync();

        ComHubConnection.Closed += OnUnintendedConnectionLoss;
    }

    /// <summary>
    /// Attempts to reconnect to backend. To be used in case of unintended loss of connection, not 
    /// on logout.
    /// </summary>
    /// <returns>
    /// A boolian success indicator.
    /// </returns>
    public static async Task<bool> Reconnect()
    {
        try
        {
            await ComHubConnection.StartAsync();
            await ComHubConnection.InvokeCoreAsync("RegisterUserIdOfConnection", new[] { Data.CurrentUser.Id });
            ComHubConnection.Closed += OnUnintendedConnectionLoss;
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Shoud be called in case of unintended loss of connection to the backend, i.e., not on logout. 
    /// Will navigate to a page that informs the user of the problem, and comes with code behind that 
    /// attempts to reconnect.
    /// </summary>
    private static Task OnUnintendedConnectionLoss(Exception exception) // TODO: What if the app is not in the frontend?
    {
        ComHubConnection.Closed -= OnUnintendedConnectionLoss;
        Application.Current.MainPage.Dispatcher.Dispatch(async () =>
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(new ConnectionLostPage());
        });
        return Task.CompletedTask;
    }

    /// <summary>
    /// Closes the connection to the backend. Should be used on logout.
    /// </summary>
    private static async Task CloseConnectionIntentionally() // TODO: Should this also be used when the app goes to the background?
    {
        ComHubConnection.Closed -= OnUnintendedConnectionLoss;
        await ComHubConnection.StopAsync();
    }

    /// <summary>
    /// Sends a user to the backend and gets all the inital data needed by the app back.
    /// </summary>
    /// 
    /// <returns>
    /// A tuple. The first element is a boolean, which is true iff the operation was successful. 
    /// If so, the third element contains the data. If not, the second element contains an error 
    /// message.
    /// </returns>
    public static async Task<(bool, string, OnLoginData)> OnLogin(User user)
    {
        try
        {
            await Connect();

            OnLoginData onLoginData = await ComHubConnection.InvokeCoreAsync<OnLoginData>("OnLogin", new[] { user });
            return (true, "", onLoginData);
        }
        catch (Exception ex)
        {
            return (false, ex.Message, null);
        }
    }

    /// <summary>
    /// Sends a user with updated wishes to the backend and gets updated swiping products back.
    /// </summary>
    /// 
    /// <returns>
    /// A tuple. The first element is a boolean, which is true iff the operation was successful. 
    /// If so, the third element contains the products. If not, the second element contains an 
    /// error message.
    /// </returns>
    public static async Task<(bool, string, Product[])> OnWishesUpdate(User user)
    {
        try
        {
            Product[] swipingProductsArray = await ComHubConnection.InvokeCoreAsync<Product[]>("OnWishesUpdate", new[] { user });
            return (true, "", swipingProductsArray);
        }
        catch (Exception ex)
        {
            return (false, ex.Message, null);
        }
    }

    /// <summary>
    /// Posts a new user product to the backend.
    /// The parameter must be of type ProductWithoutId, as the ID is supplied by the backend.
    /// </summary>
    /// 
    /// <returns>
    /// A tuple. The first element is a boolean, which is true iff the operation was successful. 
    /// If so, the third element contains the product information as saved to the database 
    /// (without image data). If not, the second element contains an error message.
    /// </returns>
    public static async Task<(bool, string, Product)> NewProduct(ProductWithoutId productWithoutId)
    {
        try
        {
            Product product = await ComHubConnection.InvokeCoreAsync<Product>("NewProduct", new[] { productWithoutId });
            return (true, "", product);
        }
        catch (Exception ex)
        {
            return (false, ex.Message, null);
        }
    }

    /// <summary>
    /// Modifies a user product in the backend.
    /// The parameter must be the entire Product.
    /// </summary>
    /// 
    /// <returns>
    /// A tuple. The first element is a boolean, which is true iff the operation was successful. 
    /// If not, the second element contains an error message.
    /// </returns>
    public static async Task<(bool, string)> ChangeProduct(Product product)
    {
        try
        {
            bool success = await ComHubConnection.InvokeCoreAsync<bool>("ChangeProduct", new[] { product });
            if (success)
            {
                return (true, "");
            }
            else
            {
                return (false, ""); // TODO: can this be improved?
            }
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    /// <summary>
    /// Deletes a user product from the backend.
    /// The parameter must be the entire Product.
    /// </summary>
    /// 
    /// <returns>
    /// A tuple. The first element is a boolean, which is true iff the operation was successful. 
    /// If not, the second element contains an error message.
    /// </returns>
    public static async Task<(bool, string)> DeleteProduct(Product product)
    {
        try
        {
            bool success = await ComHubConnection.InvokeCoreAsync<bool>("DeleteProduct", new object[] { product.Id });
            if (success)
            {
                return (true, "");
            }
            else
            {
                return (false, ""); // TODO: can this be improved?
            }
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public static async Task<(bool, string, Message)> SendMessage(Message message, string userId)
    {
        try
        {
            Message returnedMessage = await ComHubConnection.InvokeCoreAsync<Message>("SendMessage", new object[] { message, userId });
            return (true, "", returnedMessage);
        }
        catch (Exception ex)
        {
            return (false, ex.Message, null);
        }
    }

    public static async Task<Product[]?> OnSwipe(OnSwipeData onSwipeData, string swipeAction)
    {
        try
        {
            return await ComHubConnection.InvokeCoreAsync<Product[]?>(swipeAction, new[] { onSwipeData });
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static async Task<Product[]?> OnRefreshMainpage(OnRefreshMainpageData onRefreshMainpageData)
    {
        try
        {
            return await ComHubConnection.InvokeCoreAsync<Product[]?>("OnRefreshMainpage", new[] { onRefreshMainpageData });
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static async Task OnLogout()
    {
        await CloseConnectionIntentionally();
    }

    //public static readonly HttpClient client = new(); // Is this needed to get pictures? I think not.

    static readonly string ImagePartialUrl = BaseUrl + "images/";
    
    /// <summary>
    /// Returns the URL of the image of the product with the ID given in the parameter.
    /// </summary>
    public static string GetImageUrl(int Id)
        => $"{ImagePartialUrl}{Id}.jpg";
}