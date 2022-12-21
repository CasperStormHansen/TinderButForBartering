using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace TinderButForBartering;

class Data
{
    public static ObservableCollection<Product> OwnProducts { get; private set; } = new();

    public static async Task<(bool, string)> GetOwnProducts()
    {
        (bool wasSuccessful, string productsStringOrErrorInfo) = await Backend.GetProducts();

        if (wasSuccessful)
        {
            List<Product> productsList = JsonConvert.DeserializeObject<List<Product>>(productsStringOrErrorInfo);
            foreach (Product product in productsList)
            {
                OwnProducts.Add(product);
            }
            return (true, "");
        }
        return (false, productsStringOrErrorInfo);
    }

    public static async Task<(bool, string)> AddNewOwnProduct(ProductWithoutId productWithoutId)
    {
        (bool wasSuccessful, string productStringOrErrorInfo) = await Backend.PostProduct(productWithoutId);

        if (wasSuccessful)
        {
            Product product = JsonConvert.DeserializeObject<Product>(productStringOrErrorInfo);
            OwnProducts.Add(product);
            return (true, "");
        }
        return (false, productStringOrErrorInfo);

    }

    public static async Task<(bool, string)> ChangeOwnProduct(Product product)
    {
        (bool wasSuccessful, string errorInfo) = await Backend.ChangeProduct(product);

        if (wasSuccessful)
        {
            Product productToBeReplaced = OwnProducts.FirstOrDefault(p => p.Id == product.Id);
            int index = OwnProducts.IndexOf(productToBeReplaced);
            OwnProducts.RemoveAt(index);
            OwnProducts.Insert(index, product);
            return (true, "");
        }
        return (false, errorInfo);
    }

    public static async Task<(bool, string)> DeleteOwnProduct(Product product)
    {
        (bool wasSuccessful, string errorInfo) = await Backend.DeleteProduct(product);

        if (wasSuccessful) 
        {
            OwnProducts.Remove(product);
            return (true, "");
        }
        return (false, errorInfo);
    }
}
