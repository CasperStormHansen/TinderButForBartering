using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Json;
using Newtonsoft.Json;

namespace TinderButForBartering;

public class Backend
{
    public static HubConnection ComHubConnection; // change to private

    public static async Task<(bool, string, OnLoginData)> OnLogin(User user)
    {
        try 
        {
            ComHubConnection = new HubConnectionBuilder()
                .WithUrl("http://10.0.2.2:5045/comhub")
                .Build();

            await ComHubConnection.StartAsync();

            OnLoginData onLoginData = await ComHubConnection.InvokeCoreAsync<OnLoginData>("OnLogin", new[] { user });
            return (true, "", onLoginData);
        }
        catch (Exception ex)
        {
            return (false, ex.Message, null);
        }
    }

    // Above: SignalR; below: traditional http, to be changed

    public static readonly HttpClient client = new ();

#if ANDROID
    static readonly string BaseUrl = "http://10.0.2.2:5045/";
#else
    static readonly string BaseUrl = "https://localhost:7239/";
#endif

    static readonly string OnLoginUrl = BaseUrl + "onlogin/";
    static readonly string OnWishesUpdateUrl = BaseUrl + "onwishesupdate/";
    static readonly string NewProductUrl = BaseUrl + "newproduct/";
    static readonly string ChangeProductPartialUrl = BaseUrl + "changeproduct/";
    static readonly string DeleteProductPartialUrl = BaseUrl + "deleteproduct/";
    static readonly string ImagePartialUrl = BaseUrl + "images/";
    static readonly string NoToProductUrl = BaseUrl + "notoproduct/";
    static readonly string YesToProductUrl = BaseUrl + "yestoproduct/";
    static readonly string WillPayForProductUrl = BaseUrl + "willpayforproduct/";

    /// <summary>
    /// Returns the URL of the image of the product with the ID given in the parameter.
    /// </summary>
    public static string GetImageUrl(int Id)
        => $"{ImagePartialUrl}{Id}.jpg";

    /// <summary>
    /// Sends a user to the backend and gets all the inital data needed by the app back.
    /// </summary>
    /// 
    /// <returns>
    /// A tuple. The first element is a boolean, which is true iff the operation was successful. 
    /// If so, the second element contains the data as a JSON string. If not, the second element
    /// contains an error message.
    /// </returns>
    //public static async Task<(bool, string)> OnLogin(User user)
    //{
    //    try
    //    {
    //        HttpResponseMessage response = await client.PostAsJsonAsync(OnLoginUrl, user);
    //        return await ConvertReponse(response);
    //    }
    //    catch (Exception ex)
    //    {
    //        return (false, ex.Message);
    //    }
    //}

    /// <summary>
    /// Sends a user with updated wishes to the backend and gets updated swiping products back.
    /// </summary>
    /// 
    /// <returns>
    /// A tuple. The first element is a boolean, which is true iff the operation was successful. 
    /// If so, the second element contains the products as a JSON string. If not, the second element
    /// contains an error message.
    /// </returns>
    public static async Task<(bool, string)> OnWishesUpdate(User user)
    {
        try
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(OnWishesUpdateUrl, user);
            return await ConvertReponse(response);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    /// <summary>
    /// Posts a new user product to the backend.
    /// The parameter must be of type ProductWithoutId, as the ID is supplied by the backend.
    /// </summary>
    /// 
    /// <returns>
    /// A tuple. The first element is a boolean, which is true iff the operation was successful. 
    /// If so, the second element contains the product information as saved to the database as a
    /// JSON string (without image data). If not, the second element contains an error message.
    /// </returns>
    public static async Task<(bool, string)> PostProduct(ProductWithoutId productWithoutId)
    {
        try
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(NewProductUrl, productWithoutId);
            return await ConvertReponse(response);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    /// <summary>
    /// Modifies a user product in the backend.
    /// The parameter must be the entire Product.
    /// </summary>
    /// 
    /// <returns>
    /// A tuple. The first element is a boolean, which is true iff the operation was successful. 
    /// If so, the second element is empty. If not, the second element contains an error message.
    /// </returns>
    public static async Task<(bool, string)> ChangeProduct(Product product)
    {
        try
        {
            string url = ChangeProductPartialUrl + product.Id + "/";
            HttpResponseMessage response = await client.PutAsJsonAsync(url, product);
            return await ConvertReponse(response);
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
    /// If so, the second element is empty. If not, the second element contains an error message.
    /// </returns>
    public static async Task<(bool, string)> DeleteProduct(Product product)
    {
        try
        {
            string url = DeleteProductPartialUrl + product.Id + "/";
            HttpResponseMessage response = await client.DeleteAsync(url);
            return await ConvertReponse(response);
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
            await client.PostAsJsonAsync(NoToProductUrl, userProductAttitude);
        }
        catch (Exception)
        {
        }
    }

    public static async Task YesToProduct(UserProductAttitude userProductAttitude)
    {
        try
        {
            await client.PostAsJsonAsync(YesToProductUrl, userProductAttitude);
        }
        catch (Exception)
        {
        }
    }

    public static async Task WillPayForProduct(UserProductAttitude userProductAttitude)
    {
        try
        {
            await client.PostAsJsonAsync(WillPayForProductUrl, userProductAttitude);
        }
        catch (Exception)
        {
        }
    }

    private static async Task<(bool, string)> ConvertReponse(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            string jsonString = await response.Content.ReadAsStringAsync();
            return (true, jsonString);
        }
        return (false, response.StatusCode.ToString());
    }
}
