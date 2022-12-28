using System.Net.Http.Json;

namespace TinderButForBartering;

public class Backend
{
    #if ANDROID
        static readonly string BaseUrl = "http://10.0.2.2:5045/";
    #else
        static readonly string BaseUrl = "https://localhost:7239/";
    #endif

    static readonly string ProductsUrl = BaseUrl + "products/";

    public static readonly HttpClient client = new ();

    /// <summary>
    /// Returns the URL of the image of the product with the ID given in the parameter.
    /// </summary>
    public static string GetIdUrl(int Id)
    {
        return $"{ProductsUrl}images/{Id}.jpg";
    }

    /// <summary>
    /// Gets information about the user's own products from the backend.
    /// </summary>
    /// 
    /// <returns>
    /// A tuple. The first element is a boolean, which is true iff the operation was successful. 
    /// If so, the second element contains the product information as a JSON string. If not, the 
    /// second element contains an error message.
    /// </returns>
    public static async Task<(bool, string)> GetProducts()
    {
        try
        {
            HttpResponseMessage response = await client.GetAsync(ProductsUrl);
            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                return (true, jsonString);
            }
            return (false, response.StatusCode.ToString()); // is this string useful?
        }
        catch (Exception ex)
        {
            return (false, ex.Message); // should it be more than the message?
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
            HttpResponseMessage response = await client.PostAsJsonAsync(ProductsUrl, productWithoutId);
            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                return (true, jsonString);
            }
            return (false, response.StatusCode.ToString()); // is this string useful?
        }
        catch (Exception ex)
        {
            return (false, ex.Message); // should it be more than the message?
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
            string url = ProductsUrl + product.Id + "/";
            HttpResponseMessage response = await client.PutAsJsonAsync(url, product);
            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                return (true, jsonString);
            }
            return (false, response.StatusCode.ToString()); // is this string useful?
        }
        catch (Exception ex)
        {
            return (false, ex.Message); // should it be more than the message?
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
            string url = ProductsUrl + product.Id + "/";
            HttpResponseMessage response = await client.DeleteAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                return (true, jsonString);
            }
            return (false, response.StatusCode.ToString()); // is this string useful?
        }
        catch (Exception ex)
        {
            return (false, ex.Message); // should it be more than the message?
        }
    }
}
