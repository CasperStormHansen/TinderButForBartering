namespace TinderButForBartering;

public partial class MyGoodsPage : ContentPage
{
    public MyGoodsPage()
    {
        InitializeComponent();
        MyProductsView.ItemsSource = Data.OwnProducts;
    }

    private async void OnUserProductDetails_Clicked(object sender, EventArgs e)
    {
        ImageButton button = sender as ImageButton;
        Product product = button.BindingContext as Product;
        await Navigation.PushAsync(new UserProductDetailsPage(product));
    }

    private async void OnNewProduct_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new UserProductDetailsPage());
    }
}