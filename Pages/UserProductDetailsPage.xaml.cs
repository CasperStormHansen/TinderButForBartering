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
        await Update(ProductDetailsLabel);
    }

    static async Task Update(Label label)
    {
        label.Text = await GetProducts();
    }
}