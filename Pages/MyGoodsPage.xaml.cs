namespace TinderButForBartering;

public partial class MyGoodsPage : ContentPage
{
	public MyGoodsPage()
	{
		InitializeComponent();
	}
    private async void OnUserProductDetails_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new UserProductDetailsPage());
    }
}