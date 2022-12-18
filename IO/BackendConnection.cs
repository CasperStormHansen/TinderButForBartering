﻿using System.Net.Http.Json;

namespace TinderButForBartering;

public class BackendConnection
{
#if ANDROID
    static readonly string BaseUrl = "http://10.0.2.2:5045/";
#else
    static readonly string BaseUrl = "https://localhost:7239/";
#endif

    static readonly string ProductsUrl = BaseUrl + "products/";

    public static readonly HttpClient client = new();

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
    
    public static async Task<string> PostProduct(Product product) // what to do with return value?
    {
        try
        {
            HttpResponseMessage mes = await client.PostAsJsonAsync<Product>(ProductsUrl, product);
            return "mes";
        }
        catch (Exception ex)
        {
            return ex.Message; // how to handle?
        }
    }
}
