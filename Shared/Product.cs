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

        public ProductWithoutId(string title, string description, bool requiresSomethingInReturn, byte[] primaryPictureData)
        {
            OwnerId = Data.CurrentUser.Id;
            Category = 0; // to be altered
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
        public string Url => Backend.GetIdUrl(Id); // name this better!

        public Product(string title, string description, bool requiresSomethingInReturn, byte[] primaryPictureData, int id) : base(title, description, requiresSomethingInReturn, primaryPictureData)
        {
            Id = id;
        }
    }
}
