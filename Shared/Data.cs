using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Firebase.Auth;
using System.Collections.ObjectModel;

namespace TinderButForBartering;

class Data
{
    /// <summary>
    /// The current user. Is null if no one is logged in.
    /// </summary>
#nullable enable
    public static User? CurrentUser { get; set; }
#nullable disable

    /// <summary>
    /// The user's own products.
    /// </summary>
    public static ObservableCollection<Product> OwnProducts { get; private set; } = new();

    /// <summary>
    /// The products in the swipe stack.
    /// </summary>
    public static ObservableCollection<Product> SwipingProducts { get; private set; } = new();

    /// <summary>
    /// Attempts to get information needed at login (incl. app start when user is still logged in) via the backend
    /// class, deserilializes it, and stores it in this class's properties.
    /// </summary>
    /// 
    /// <returns>
    /// A tuple. The first element is a boolian that indicates if the operation was successful. If not, the second
    /// element contains an error message.
    /// </returns>
    public static async Task<(bool, string)> OnLogin(IFirebaseUser firebaseUser)
    {
        CurrentUser = new User(firebaseUser);

        (bool success, string infoStringOrError) = await Backend.OnLogin(CurrentUser);
        await App.Current.MainPage.DisplayAlert("infoStringOrError == ", infoStringOrError, "OK"); // to be deleted; for development only

        if (success)
        {
            OnLoginData onLoginData = JsonConvert.DeserializeObject<OnLoginData>(infoStringOrError);

            CurrentUser = onLoginData.item1;
            foreach (Product product in onLoginData.item2) OwnProducts.Add(product);
            foreach (Product product in onLoginData.item3) SwipingProducts.Add(product);

            return (true, "");
        }
        return (false, infoStringOrError);
    }

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
        (bool success, string productsStringOrErrorInfo) = await Backend.GetProducts();

        if (success)
        {
            List<Product> productsList = JsonConvert.DeserializeObject<List<Product>>(productsStringOrErrorInfo);
            foreach (Product product in productsList) OwnProducts.Add(product);

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
        (bool success, string productStringOrErrorInfo) = await Backend.PostProduct(productWithoutId);

        if (success)
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
        (bool success, string errorInfo) = await Backend.ChangeProduct(product);

        if (success)
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
        (bool success, string errorInfo) = await Backend.DeleteProduct(product);

        if (success) 
        {
            OwnProducts.Remove(product);

            return (true, "");
        }
        return (false, errorInfo);
    }

    /// <summary>
    /// Deletes the local data about the user and products.
    /// </summary>
    public static void DeleteLocalData()
    {
        CurrentUser = null;
        OwnProducts.Clear();
        SwipingProducts.Clear();
    }
}

/// <summary>
/// Helper class only used internally in the OnLogin method for deserialization.
/// </summary>
class OnLoginData
{
    public User item1 { get; set; }
    public List<Product> item2 { get; set; }
    public List<Product> item3 { get; set; }
}