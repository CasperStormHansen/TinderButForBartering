using Newtonsoft.Json;
//using System.Diagnostics;
//using System.Text.Json;
//using Windows.System;
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

    //async Task Update()
    //{
    //    String productsString = await GetProducts();
    //    Debug.WriteLine(productsString);
    //    //var products = JsonSerializer.Deserialize<IList<ProductWithId>>(productsString); // async?
    //    var product = JsonSerializer.Deserialize<ProductWithId>(productsString);
    //    //Data.Text = products[0].Description;
    //    Data.Text = product.Description;
    //}

    async Task Update()
    {
        String productsString = await GetProducts();
        Product product = JsonConvert.DeserializeObject<Product>(productsString);
        Data.Text = product.Description;
    }
}