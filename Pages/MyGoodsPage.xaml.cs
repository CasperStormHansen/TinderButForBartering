using static TinderButForBartering.Data;

namespace TinderButForBartering;

public partial class MyGoodsPage : ContentPage
{
    public MyGoodsPage()
    {
        InitializeComponent();
        MyProductsView.ItemsSource = OwnProducts;
        //_ = GetOwnProducts();
    }

    //protected override async void OnAppearing()
    //{
    //    base.OnAppearing();
    //}

    private async void OnUserProductDetails_Clicked(object sender, EventArgs e)
    {
        Button button = sender as Button;
        Product product = button.BindingContext as Product;
        await Navigation.PushAsync(new UserProductDetailsPage(product));
    }

    private async void OnNewProduct_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new UserProductDetailsPage());
    }
}