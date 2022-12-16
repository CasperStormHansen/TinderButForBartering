using static TinderButForBartering.BackendConnection;

namespace TinderButForBartering;

public partial class UserProductDetailsPage : ContentPage
{
    public UserProductDetailsPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Update();
    }

    async Task Update()
    {
        ProductDetailsLabel.Text = await GetProducts();
    }

    async void OnAddProduct_Clicked(object sender, EventArgs e)
    {
        Product product = new (Titel.Text, Description.Text, Switch.IsToggled);
        await PostProduct(product);
    }
}