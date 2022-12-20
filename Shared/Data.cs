using Newtonsoft.Json;
using System.Collections.ObjectModel;
using static TinderButForBartering.BackendConnection;

namespace TinderButForBartering;

class Data
{
    public static ObservableCollection<Product> OwnProducts { get; private set; } = new();

    public static async Task GetOwnProducts()
    {
        string productsString = await GetProducts();
        List<Product> productsList = JsonConvert.DeserializeObject<List<Product>>(productsString);
        foreach (Product product in productsList)
        {
            OwnProducts.Add(product);
        }
    }

    public static async void AddNewOwnProduct(ProductWithoutId productWithoutId)
    {
        string productString = await PostProduct(productWithoutId);

        Product product = JsonConvert.DeserializeObject<Product>(productString);
        OwnProducts.Add(product);
    }

    public static async void ChangeOwnProduct(Product product)
    {
        await ChangeProduct(product); // only proceed if successful

        int index = OwnProducts.IndexOf(OwnProducts.FirstOrDefault(p => p.Id == product.Id));
        OwnProducts.RemoveAt(index);
        OwnProducts.Insert(index, product);
    }

    public static async void DeleteOwnProduct(Product product)
    {
        await DeleteProduct(product);

        OwnProducts.Remove(product);
    }
}
