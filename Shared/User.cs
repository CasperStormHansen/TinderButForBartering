using Plugin.Firebase.Auth;
using Newtonsoft.Json;
using System.Collections;

namespace TinderButForBartering;

public class User
{
    public string Id { get; set; }
    public string Name { get; set; }
#nullable enable
    public string? PictureUrl { get; set; }
    public byte[]? Wishlist { get; set; } // array of the indexies of the categories the user is interested in

    [JsonConstructor]
    public User(string id, string name, string? pictureUrl)
#nullable disable
    {
        Id = id;
        Name = name;
        PictureUrl = pictureUrl;
    }

    public User(IFirebaseUser firebaseUser)
    {
        Id = firebaseUser.Uid;
        Name = firebaseUser.DisplayName;
        PictureUrl = firebaseUser.PhotoUrl;
    }
}
