using Plugin.Firebase.Auth;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

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
    /// Determines when more swiping products will be requested from the backend.
    /// </summary>
    private static readonly int MinSwipingProducts = 5; // Should be half of MaxSwipingProducts on backend. Could be transfered as part of OnLoginData.
    
    /// <summary>
    /// A new request for swiping products will not be initiated while this is true.
    /// </summary>
    private static bool RequestForSwipingProductsIsUnderWay { get; set; } = false;

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
            CurrentUser = onLoginData.User;
            foreach (Product product in onLoginData.OwnProducts) OwnProducts.Add(product);
            foreach (Product product in onLoginData.SwipingProducts) SwipingProducts.Enqueue(product);
            Categories = onLoginData.Categories;
            foreach (Match match in onLoginData.Matches) Matches.Add(match);

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

    public static async Task<(bool, string)> SendMessage(Match match, string messageContent)
    {
        Message message = new(match.MatchId, true, messageContent, null);

        (bool success, string errorInfo, Message returnedMessage) = await Backend.SendMessage(message, CurrentUser.Id);

        if (success)
        {
            match.Messages.Add(returnedMessage);
            return (true, "");
        }
        return (false, errorInfo);
    }

    public static void ReceiveMessage(Message message)
    {
        Match match = Matches.FirstOrDefault(m => m.MatchId == message.MatchId);
        match.Messages.Add(message);
    }

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public static async Task OnSwipe(string swipeAction)
    {
        Product product = SwipingProducts.Dequeue();
        BackgroundProcessingAfterSwipe(swipeAction, product);
    }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

    private static async Task BackgroundProcessingAfterSwipe(string swipeAction, Product product)
    {
        UserProductAttitude userProductAttitude = new(CurrentUser, product);
        OnSwipeData? onSwipeData;
        if (SwipingProducts.Count <= MinSwipingProducts && !RequestForSwipingProductsIsUnderWay)
        {
            RequestForSwipingProductsIsUnderWay = true;
            int[] remainingSwipingProductIds = SwipingProducts.Select(p => p.Id).ToArray();
            onSwipeData = new (userProductAttitude, remainingSwipingProductIds);
        }
        else
        {
            onSwipeData = new (userProductAttitude, null);
        }

        Product[]? extraProducts = await Backend.OnSwipe(onSwipeData, swipeAction);

        if (extraProducts != null)
        {
            foreach (Product extraProduct in extraProducts)
            {
                SwipingProducts.Enqueue(extraProduct);
            }
        }

        RequestForSwipingProductsIsUnderWay = false;
    }

    public static void ReceiveMatch(Match match)
    {
        Matches.Add(match);
    }

    public static void AddForeignProductToMatch(Product product, int matchId)
    {
        Match match = Matches.FirstOrDefault(m => m.MatchId == matchId);
        match?.ForeignProducts.Add(product);
    }

    public static void AddOwnProductToMatch(int productId, int matchId)
    {
        Match match = Matches.FirstOrDefault(m => m.MatchId == matchId);
        match?.OwnProductIds.Add(productId);
    }

    public static void UpdateForeignProductInMatch(Product product, int matchId)
    {
        Match match = Matches.FirstOrDefault(m => m.MatchId == matchId);
        for (int i = 0; i < match.ForeignProducts.Count; i++)
        {
            if (match.ForeignProducts[i].Id == product.Id)
            {
                match.ForeignProducts[i] = product;
                break;
            }
        }
    }

    /// <summary>
    /// Deletes the local data about the user and products.
    /// </summary>
    public static async void Logout()
    {
        CurrentUser = null;
        OwnProducts.Clear();
        SwipingProducts.Clear();
        Categories = null;
        Matches.Clear();

        await Backend.OnLogout();
    }
}