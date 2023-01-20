using System.Text.Json.Serialization;

namespace TinderButForBartering
{
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
}
