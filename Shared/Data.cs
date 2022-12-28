using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace TinderButForBartering;

class Data
{
    /// <summary>
    /// A collection of the user's own products.
    /// </summary>
    public static ObservableCollection<Product> OwnProducts { get; private set; } = new();

    /// <summary>
    /// Attempts to get information about the user's own products via the backend class, deserilializes it, and
    /// stores it in OwnProducts.
    /// </summary>
    /// 
    /// <returns>
    /// A tuple. The first element is a boolian that indicates if the operation was successful. If not, the second
    /// element contains an error message.
    /// </returns>
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

    /// <summary>
    /// Attempts to send information about a new user-owned product via the backend class and also stores it in
    /// OwnProducts.
    /// </summary>
    /// 
    /// <returns>
    /// A tuple. The first element is a boolian that indicates if the operation was successful. If not, the second
    /// element contains an error message.
    /// </returns>
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

    /// <summary>
    /// Attempts to send information about an amended user-owned product via the backend class and also updates
    /// OwnProducts.
    /// </summary>
    /// 
    /// <returns>
    /// A tuple. The first element is a boolian that indicates if the operation was successful. If not, the second
    /// element contains an error message.
    /// </returns>
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

    /// <summary>
    /// Attempts to send information about a user-owned product to be deleted via the backend class and also updates
    /// OwnProducts.
    /// </summary>
    /// 
    /// <returns>
    /// A tuple. The first element is a boolian that indicates if the operation was successful. If not, the second
    /// element contains an error message.
    /// </returns>
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
