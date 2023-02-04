namespace TinderButForBartering;

public partial class ForeignProductsDetailsPage : ContentPage
{
	private bool FromMainPage { get; set; }

	public ForeignProductsDetailsPage(Product product, bool fromMainPage)
	{
		InitializeComponent();

		ProductTitle.Text = product.Title;
		Category.Text = Data.Categories[product.Category];
		Description.Text = product.Description;
		PrimaryPicture.Source = product.Url;

		FromMainPage = fromMainPage;
	}

    async void OnBackButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
		if (FromMainPage)
		{
			Data.MainPage.OnAnyAppearance();
		}
    }
}