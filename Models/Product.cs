namespace TinderButForBartering
{
    public class Product
    {
        public string ProductTitle { get; set; }
        public string Description { get; set; }
        public bool RequiresSomethingInReturn { get; set; }

        public Product(string title, string description, bool requiresSomethingInReturn)
        {
            ProductTitle = title;
            Description = description;
            RequiresSomethingInReturn = requiresSomethingInReturn;
        }
    }
}
