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

    public static HubConnection ComHubConnection { get; set; } // change to private

    private static async Task Connect()
    {
        ComHubConnection = new HubConnectionBuilder()
                .WithUrl(ComHubUrl)
                .Build();

        await ComHubConnection.StartAsync();
        // TODO: error handling
    }

    private static async Task EnsureConnection()
    {
        if (ComHubConnection.State == HubConnectionState.Disconnected) // TODO: seems to disconnect quickly. Why is that?
        {
            //ComHubConnection = new HubConnectionBuilder()
            //    .WithUrl(ComHubUrl)
            //    .Build();

            await ComHubConnection.StartAsync();
            // TODO: error handling
        }
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
            await Connect(); // TODO: Close on logout

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
        await EnsureConnection();

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
        await EnsureConnection();

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
            await EnsureConnection();

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
            await EnsureConnection();

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

    public static async Task NoToProduct(UserProductAttitude userProductAttitude)
    {
        try
        {
            await ComHubConnection.InvokeCoreAsync("NoToProduct", new[] { userProductAttitude });
        }
        catch (Exception)
        {
        }
    }

    public static async Task YesToProduct(UserProductAttitude userProductAttitude)
    {
        try
        {
            await ComHubConnection.InvokeCoreAsync("YesToProduct", new[] { userProductAttitude });
        }
        catch (Exception)
        {
        }
    }

    public static async Task WillPayForProduct(UserProductAttitude userProductAttitude)
    {
        try
        {
            await ComHubConnection.InvokeCoreAsync("WillPayForProduct", new[] { userProductAttitude });
        }
        catch (Exception)
        {
        }
    }

    public static readonly HttpClient client = new(); // Is this needed to get pictures? I think not.

    static readonly string ImagePartialUrl = BaseUrl + "images/";
    
    /// <summary>
    /// Returns the URL of the image of the product with the ID given in the parameter.
    /// </summary>
    public static string GetImageUrl(int Id)
        => $"{ImagePartialUrl}{Id}.jpg";
}