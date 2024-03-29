﻿//using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace TinderButForBartering;

public class ProductWithoutId
{
    public string OwnerId { get; set; }
    public byte Category { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool RequiresSomethingInReturn { get; set; }
#nullable enable
    public byte[]? PrimaryPictureData { get; set; }
#nullable disable

    public ProductWithoutId(string title, byte category, string description, bool requiresSomethingInReturn, byte[] primaryPictureData)
    {
        OwnerId = Data.CurrentUser.Id;
        Category = category;
        Title = title;
        Description = description;
        RequiresSomethingInReturn = requiresSomethingInReturn;
        PrimaryPictureData = primaryPictureData;
    }
}

public class Product : ProductWithoutId
{
    public int Id { get; set; }

    [JsonIgnore]
    public string Url => Backend.GetImageUrl(Id); // name this better!

    public Product(string title, byte category, string description, bool requiresSomethingInReturn, byte[] primaryPictureData, int id) : base(title, category, description, requiresSomethingInReturn, primaryPictureData)
    {
        Id = id;
    }
}

#nullable enable
public class User
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string? PictureUrl { get; set; }
    public byte[]? Wishlist { get; set; } // array of the indexies of the categories the user is interested in

    [JsonConstructor]
    public User(string id, string name, string? pictureUrl, byte[]? wishlist)
    {
        Id = id;
        Name = name;
        PictureUrl = pictureUrl;
        Wishlist = wishlist;
    }
}
#nullable disable

public class Match
{
    public int MatchId { get; set; }
    public string Name { get; set; }
#nullable enable
    public string? PictureUrl { get; set; }
#nullable disable
    //public int[] OwnProductIds { get; set; }
    [JsonConverter(typeof(ObservableCollectionConverter<int>))]
    public ObservableCollection<int> OwnProductIds { get; set; }
    [JsonConverter(typeof(ObservableCollectionConverter<Product>))]
    public ObservableCollection<Product> ForeignProducts { get; set; }
    [JsonConverter(typeof(ObservableCollectionConverter<Message>))]
    public ObservableCollection<Message> Messages { get; set; }
}

public class Message
{
    public int MatchId { get; set; }
    public bool Own { get; set; }
    public string Content { get; set; }
    public DateTime? DateTime { get; set; }

    [JsonConstructor]
    public Message(int matchId, bool own, string content, DateTime? dateTime)
    {
        MatchId = matchId;
        Own = own;
        Content = content;
        DateTime = dateTime;
    }
}

public class OnLoginData // Optimization: ObservableCollections instead of arrays already here
{
    public User User { get; set; }
    public Product[] OwnProducts { get; set; }
    public Product[] SwipingProducts { get; set; }
    public string[] Categories { get; set; }
    public Match[] Matches { get; set; }

    [JsonConstructor]
    public OnLoginData(User user, Product[] ownProducts, Product[] swipingProducts, string[] categories, Match[] matches)
    {
        User = user;
        OwnProducts = ownProducts;
        SwipingProducts = swipingProducts;
        Categories = categories;
        Matches = matches;
    }
}

public class OnReconnectionData
{
    public Match[] NewMatches { get; set; }
    public Product[] UpdatedForeignProducts { get; set; }
    public MatchIdAndProductId[] NewInterestsInOwnProducts { get; set; }
    public Message[] NewMessages { get; set; }

    [JsonConstructor]
    public OnReconnectionData(Match[] newMatches, Product[] updatedForeignProducts, MatchIdAndProductId[] newInterestsInOwnProducts, Message[] newMessages)
    {
        NewMatches = newMatches;
        UpdatedForeignProducts = updatedForeignProducts;
        NewInterestsInOwnProducts = newInterestsInOwnProducts;
        NewMessages = newMessages;
    }
}

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

#nullable enable
public class OnSwipeData
{
    public UserProductAttitude UserProductAttitude { get; set; }
    public int[]? RemainingSwipingProductIds { get; set; }

    [JsonConstructor]
    public OnSwipeData(UserProductAttitude userProductAttitude, int[]? remainingSwipingProductIds)
    {
        UserProductAttitude = userProductAttitude;
        RemainingSwipingProductIds = remainingSwipingProductIds;
    }
}
#nullable disable

public class OnRefreshMainpageData
{
    public string UserId { get; set; }
    public int[] RemainingSwipingProductIds { get; set; }

    [JsonConstructor]
    public OnRefreshMainpageData(string userId, int[] remainingSwipingProductIds)
    {
        UserId = userId;
        RemainingSwipingProductIds = remainingSwipingProductIds;
    }
}

public class UserAndLastUpdate
{
    public string UserId { get; set; }
    public DateTime LastUpdate { get; set; }

    [JsonConstructor]
    public UserAndLastUpdate(string userId, DateTime lastUpdate)
    {
        UserId = userId;
        LastUpdate = lastUpdate; 
    }
}

public class MatchIdAndProductId
{
    public int MatchId { get; set; }
    public int ProductId { get; set; }

    [JsonConstructor]
    public MatchIdAndProductId(int matchId, int productId)
    {
        MatchId = matchId;
        ProductId = productId;
    }
}

public class TimeStamped<T>
{
    public T Value { get; set; }
    public DateTime SendTime { get; set; }

    [JsonConstructor]
    public TimeStamped(T value, DateTime sendTime)
    {
        Value = value;
        SendTime = sendTime;
    }
}