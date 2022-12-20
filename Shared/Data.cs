using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace TinderButForBartering;

class Data
{
    public static ObservableCollection<Product> OwnProducts { get; private set; } = new();

    public static async Task GetOwnProducts()
    {
        string productsString = await Backend.GetProducts();

        List<Product> productsList = JsonConvert.DeserializeObject<List<Product>>(productsString);
        foreach (Product product in productsList)
        {
            OwnProducts.Add(product);
        }
    }

    public static async void AddNewOwnProduct(ProductWithoutId productWithoutId)
    {
        string productString = await Backend.PostProduct(productWithoutId);

        Product product = JsonConvert.DeserializeObject<Product>(productString);
        OwnProducts.Add(product);
    }

    public static async void ChangeOwnProduct(Product product)
    {
        await Backend.ChangeProduct(product); // only proceed if successful

        Product productToBeReplaced = OwnProducts.FirstOrDefault(p => p.Id == product.Id);
        int index = OwnProducts.IndexOf(productToBeReplaced);
        OwnProducts.RemoveAt(index);
        OwnProducts.Insert(index, product);
    }

    public static async void DeleteOwnProduct(Product product)
    {
        await Backend.DeleteProduct(product);

        OwnProducts.Remove(product);
    }
}
