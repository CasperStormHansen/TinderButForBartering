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

    public static string GetIdUrl(int Id)
    {
        return $"{ProductsUrl}images/{Id}.jpg";
    }

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
