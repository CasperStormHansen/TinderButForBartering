using Newtonsoft.Json;
using Plugin.Firebase.Auth;
using System.Collections.ObjectModel;

namespace TinderButForBartering;

class Data
{
#nullable enable
    /// <summary>
    /// Will hold a reference to the app's mainpage after it has been initialized.
    /// </summary>
    public static MainPage? MainPage { get; set; }

    /// <summary>
    /// The current user. Is null if no one is logged in.
    /// </summary>
    public static User? CurrentUser { get; set; }
#nullable disable

    /// <summary>
    /// The user's own products.
    /// </summary>
    public static ObservableCollection<Product> OwnProducts { get; set; } = new();

    /// <summary>
    /// The products in the swipe stack.
    /// </summary>
    public static Queue<Product> SwipingProducts { get; set; } = new();

    /// <summary>
    /// The product categories.
    /// </summary>
    public static string[] Categories { get; set; }

    /// <summary>
    /// The matches with other user (including data about products and chat messages).
    /// </summary>
    public static ObservableCollection<Match> Matches { get; set; } = new();

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
        CurrentUser = new User(firebaseUser.Uid, firebaseUser.DisplayName, firebaseUser.PhotoUrl, null);

        (bool success, string errorMessage, OnLoginData onLoginData) = await Backend.OnLogin(CurrentUser);

        if (success)
        {
            CurrentUser = onLoginData.Item1;
            foreach (Product product in onLoginData.Item2) OwnProducts.Add(product);
            foreach (Product product in onLoginData.Item3) SwipingProducts.Enqueue(product);
            Categories = onLoginData.Item4;
            foreach (Match match in onLoginData.Item5) Matches.Add(match);

            return (true, "");
        }
        return (false, errorMessage);
    }

    /// <summary>
    /// Attempts to send updated wishlist to backend and get an updated list of products for swiping in return, which 
    /// is then deserilialized and stored it in this class's SwipingProducts property.
    /// </summary>
    /// 
    /// <returns>
    /// A tuple. The first element is a boolian that indicates if the operation was successful. If not, the second
    /// element contains an error message.
    /// </returns>
    public static async Task<(bool, string)> OnWishesUpdate(byte[] wishlist)
    {
        CurrentUser.Wishlist = wishlist;
        (bool success, string errorMessage, Product[] swipingProductsArray) = await Backend.OnWishesUpdate(CurrentUser);
        
        if (success)
        {
            SwipingProducts.Clear();
            foreach (Product product in swipingProductsArray) SwipingProducts.Enqueue(product);
            
            return (true, "");
        }
        return (false, errorMessage);
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
        (bool success, string errorInfo, Product product) = await Backend.NewProduct(productWithoutId);

        if (success)
        {
            OwnProducts.Add(product);

            return (true, "");
        }
        return (false, errorInfo);

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

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public static async Task NoToProduct()
    {
        Product product = SwipingProducts.Dequeue();
        UserProductAttitude userProductAttitude = new(CurrentUser, product);
        Backend.NoToProduct(userProductAttitude);
    }

    public static async Task YesToProduct()
    {
        Product product = SwipingProducts.Dequeue();
        UserProductAttitude userProductAttitude = new(CurrentUser, product);
        Backend.YesToProduct(userProductAttitude);
    }

    public static async Task WillPayForProduct()
    {
        Product product = SwipingProducts.Dequeue();
        UserProductAttitude userProductAttitude = new(CurrentUser, product);
        Backend.WillPayForProduct(userProductAttitude);
    }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

    /// <summary>
    /// Deletes the local data about the user and products.
    /// </summary>
    public static void DeleteLocalData()
    {
        CurrentUser = null;
        OwnProducts.Clear();
        SwipingProducts.Clear();
        Categories = null;
    }
}

/// <summary>
/// Helper class only used in the OnLogin methods for deserialization.
/// </summary>
public class OnLoginData
{
    public User Item1 { get; set; }
    public Product[] Item2 { get; set; }
    public Product[] Item3 { get; set; }
    public string[] Item4 { get; set; }
    public Match[] Item5 { get; set; }

    [JsonConstructor]
    public OnLoginData(User item1, Product[] item2, Product[] item3, string[] item4, Match[] item5)
    {
        Item1 = item1;
        Item2 = item2;
        Item3 = item3;
        Item4 = item4;
        Item5 = item5;
    }
}

/// <summary>
/// Helper class only used to send swipe info to backend.
/// </summary>
public class UserProductAttitude
{
    public string UserId { get; set; }
    public int ProductId { get; set; }

    public UserProductAttitude(User user, Product product)
    {
        UserId = user.Id;
        ProductId = product.Id;
    }
}