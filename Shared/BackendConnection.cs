using System.Net.Http.Json;

namespace TinderButForBartering;

public class BackendConnection
{
    #if ANDROID
        static readonly string BaseUrl = "http://10.0.2.2:5045/";
    #else
        static readonly string BaseUrl = "https://localhost:7239/";
    #endif

    static readonly string ProductsUrl = BaseUrl + "products/";

    public static readonly HttpClient client = new ();

    public static async Task<string> GetProducts()
    {
        try
        {
            return await client.GetStringAsync(ProductsUrl);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    
    public static async Task<string> PostProduct(ProductWithoutId productWithoutId)
    {
        try
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(ProductsUrl, productWithoutId);
            string jsonString = await response.Content.ReadAsStringAsync();
            return jsonString; // success assumed response.IsSuccessStatusCode
        }
        catch (Exception ex)
        {
            return ex.Message; // how to handle?
        }
    }

    public static async Task<string> ChangeProduct(Product product)
    {
        try
        {
            string url = ProductsUrl + product.Id + "/";
            HttpResponseMessage response = await client.PutAsJsonAsync(url, product);
            //string jsonString = await response.Content.ReadAsStringAsync();
            //return jsonString; // success assumed response.IsSuccessStatusCode
            return "";
        }
        catch (Exception ex)
        {
            return ex.Message; // how to handle?
        }
    }

    public static async Task<string> DeleteProduct(Product product)
    {
        try
        {
            string url = ProductsUrl + product.Id + "/";
            HttpResponseMessage response = await client.DeleteAsync(url);
            //string jsonString = await response.Content.ReadAsStringAsync();
            //return jsonString; // success assumed response.IsSuccessStatusCode
            return "";
        }
        catch (Exception ex)
        {
            return ex.Message; // how to handle?
        }
    }
}
