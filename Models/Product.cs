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
}
