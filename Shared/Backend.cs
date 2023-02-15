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
    /// The last time an update was received from the backend. The value must arrive as part of that data.
    /// </summary>
    private static DateTime LastUpdate { get; set; }

    /// <summary>
    /// Attempts to create SignalR connection with backend. Only to be called from OnLogin, which
    /// handles registration and errors.
    /// </summary>
    private static async Task Connect()
    {
        ComHubConnection = new HubConnectionBuilder()
            .WithUrl(ComHubUrl)
            .Build();

        ComHubConnection.On<DateTime, Message>("ReceiveMessage", (time, message) =>
        {
            LastUpdate = time;
            Data.ReceiveMessage(message);
        });

        ComHubConnection.On<DateTime, Match>("ReceiveMatch", (time, match) =>
        {
            LastUpdate = time;
            Data.ReceiveMatch(match);
        });

        ComHubConnection.On<DateTime, Product, int>("AddForeignProductToMatch", (time, product, matchId) =>
        {
            LastUpdate = time;
            Data.AddForeignProductToMatch(product, matchId);
        });

        ComHubConnection.On<DateTime, int, int>("AddOwnProductToMatch", (time, productId, matchId) =>
        {
            LastUpdate = time;
            Data.AddOwnProductToMatch(productId, matchId);
        });

        ComHubConnection.On<DateTime, Product, int>("UpdateForeignProductInMatch", (time, product, matchId) =>
        {
            LastUpdate = time;
            Data.UpdateForeignProductInMatch(product, matchId);
        });

        await ComHubConnection.StartAsync();

        ComHubConnection.Closed += OnUnintendedConnectionLoss;
    }

    /// <summary>
    /// Attempts to reconnect to backend. To be used in case of unintended loss of connection, not 
    /// on logout.
    /// </summary>
    /// <returns>
    /// A boolian success indicator and an update on changes that have happened since the connection
    /// was interrupted.
    /// </returns>
    public static async Task<(bool, OnReconnectionData)> Reconnect(string userId)
    {
        UserAndLastUpdate userAndLastUpdate = new(userId, LastUpdate);

        try
        {
            await ComHubConnection.StartAsync();
            OnReconnectionData onReconnectionData = await CommunicateAndDealWithTimeStamp<OnReconnectionData>("OnReconnection", new[] { userAndLastUpdate });
            ComHubConnection.Closed += OnUnintendedConnectionLoss;
            return (true, onReconnectionData);
        }
        catch
        {
            return (false, null);
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

#nullable enable
    private static async Task<T> CommunicateAndDealWithTimeStamp<T>(string backendMethod, object?[] args)
    {
        TimeStamped<T> response = await ComHubConnection.InvokeCoreAsync<TimeStamped<T>>(backendMethod, args);
        LastUpdate = response.SendTime;
        return response.Value;
    }
#nullable disable

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

            OnLoginData onLoginData = await CommunicateAndDealWithTimeStamp<OnLoginData>("OnLogin", new[] { user });
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
            Product[] swipingProductsArray = await CommunicateAndDealWithTimeStamp<Product[]>("OnWishesUpdate", new[] { user });
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
            Product product = await CommunicateAndDealWithTimeStamp<Product>("NewProduct", new[] { productWithoutId });
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
            bool success = await CommunicateAndDealWithTimeStamp<bool>("ChangeProduct", new[] { product });
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
            bool success = await CommunicateAndDealWithTimeStamp<bool>("DeleteProduct", new object[] { product.Id });
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
            Message returnedMessage = await CommunicateAndDealWithTimeStamp<Message>("SendMessage", new object[] { message, userId });
            return (true, "", returnedMessage);
        }
        catch (Exception ex)
        {
            return (false, ex.Message, null);
        }
    }

#nullable enable
    public static async Task<Product[]?> OnSwipe(OnSwipeData onSwipeData, string swipeAction)
    {
        try
        {
            return await CommunicateAndDealWithTimeStamp<Product[]?>(swipeAction, new[] { onSwipeData });
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
            return await CommunicateAndDealWithTimeStamp<Product[]?>("OnRefreshMainpage", new[] { onRefreshMainpageData });
        }
        catch (Exception)
        {
            return null;
        }
    }
#nullable disable

    public static async Task OnLogout()
    {
        await CloseConnectionIntentionally();
    }

    static readonly string ImagePartialUrl = BaseUrl + "images/";
    
    /// <summary>
    /// Returns the URL of the image of the product with the ID given in the parameter.
    /// </summary>
    public static string GetImageUrl(int Id)
        => $"{ImagePartialUrl}{Id}.jpg";
}