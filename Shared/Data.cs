using Newtonsoft.Json;
using System.Collections.ObjectModel;
using static TinderButForBartering.BackendConnection;

namespace TinderButForBartering;

class Data
{
    public static ObservableCollection<Product> Products { get; set; } = new(); // make set private

    public static async Task GetOwnProducts()
    {
        string productsString = await GetProducts();
        List<Product> productsList = JsonConvert.DeserializeObject<List<Product>>(productsString);
        foreach (Product product in productsList) // is there a more elegant way?
        {
            Products.Add(product);
        }
    }

    public static void AddNewOwnProduct(string productString)
    {
        System.Diagnostics.Debug.WriteLine(productString);
        Product product = JsonConvert.DeserializeObject<Product>(productString);
        Products.Add(product);
    }
}
