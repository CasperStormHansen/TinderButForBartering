namespace TinderButForBartering
{
    public class BackendConnection
    {
        public static readonly HttpClient client = new();

        public static async Task<string> GetProducts()
        {
            try
            {
                return await client.GetStringAsync(ProductUrl);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

#if ANDROID
        static string ProductUrl = "http://10.0.2.2:5045/products/";
#else
        static string ProductUrl = "https://localhost:7239/products/";
#endif
    }
}
