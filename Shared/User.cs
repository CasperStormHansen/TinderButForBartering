using Plugin.Firebase.Auth;
using Newtonsoft.Json;

namespace TinderButForBartering
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
#nullable enable
        public string? PictureUrl { get; set; }

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
}
