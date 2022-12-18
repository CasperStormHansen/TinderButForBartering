using Newtonsoft.Json;
using static TinderButForBartering.BackendConnection;

namespace TinderButForBartering;

public partial class MyGoodsPage : ContentPage
{
    public MyGoodsPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Update();
    }

    private async void OnUserProductDetails_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new UserProductDetailsPage());
    }

    async Task Update()
    {
        string productsString = await GetProducts();
        List<Product> products = JsonConvert.DeserializeObject<List<Product>>(productsString);
        Data.Text = products[0].Description;
    }
}