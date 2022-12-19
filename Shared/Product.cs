using System.Text.Json.Serialization;

namespace TinderButForBartering
{
    public class Product
    {
        public string ProductTitle { get; set; }
        public string Description { get; set; }
        public bool RequiresSomethingInReturn { get; set; }
        public byte[] PrimaryPictureData { get; set; }

        public Product(string title, string description, bool requiresSomethingInReturn, byte[] primaryPictureData)
        {
            ProductTitle = title;
            Description = description;
            RequiresSomethingInReturn = requiresSomethingInReturn;
            PrimaryPictureData = primaryPictureData;
        }
    }

    //public class ProductWithId
    //{
    //    [JsonPropertyName("id")]
    //    public int Id { get; set; }

    //    [JsonPropertyName("productTitle")]
    //    public string ProductTitle { get; set; }

    //    [JsonPropertyName("description")]
    //    public string Description { get; set; }

    //    [JsonPropertyName("requiresSomethingInReturn")]
    //    public bool RequiresSomethingInReturn { get; set; }

    //    //[JsonPropertyName("primaryPictureData")]
    //    //public byte[] PrimaryPictureData { get; set; }

    //    [JsonConstructor]
    //    public ProductWithId(int id, string title, string description, bool requiresSomethingInReturn)//, byte[] primaryPictureData)
    //    {
    //        Id = id;
    //        ProductTitle = title;
    //        Description = description;
    //        RequiresSomethingInReturn = requiresSomethingInReturn;
    //        //PrimaryPictureData = primaryPictureData;
    //    }
    //}
}
