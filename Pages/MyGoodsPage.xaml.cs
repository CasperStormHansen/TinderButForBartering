using Newtonsoft.Json;
using System.Collections.ObjectModel;
using static TinderButForBartering.BackendConnection;

namespace TinderButForBartering;

public partial class MyGoodsPage : ContentPage
{
    public ObservableCollection<Product> Products { get; set; } = new ();

    public MyGoodsPage()
    {
        InitializeComponent();
        MyProductsView.ItemsSource = Products;
        _ = Update();
    }

    //protected override async void OnAppearing()
    //{
    //    base.OnAppearing();
    //    //await Update();
    //    //_ = Update();
    //}

    async Task Update()
    {
        string productsString = await GetProducts();
        List<Product> productsList = JsonConvert.DeserializeObject<List<Product>>(productsString);
        foreach (Product product in productsList) // is there a more elegant way?
        {
            Products.Add(product);
        }
    }

        private async void OnUserProductDetails_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new UserProductDetailsPage());
    }
}