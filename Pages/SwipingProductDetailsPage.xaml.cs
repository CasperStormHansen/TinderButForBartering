namespace TinderButForBartering;

public partial class SwipingProductDetailsPage : ContentPage
{
	private Product Product = Data.SwipingProducts.Peek();

	public SwipingProductDetailsPage()
	{
		InitializeComponent();

		ProductTitle.Text = Product.Title;
		Category.Text = Data.Categories[Product.Category];
		Description.Text = Product.Description;
		PrimaryPicture.Source = Product.Url;
	}

    async void OnBackButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
        Data.MainPage.OnAnyAppearance();
    }
}