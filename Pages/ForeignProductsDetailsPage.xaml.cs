namespace TinderButForBartering;

public partial class ForeignProductsDetailsPage : ContentPage
{
	public ForeignProductsDetailsPage(Product product)
	{
		InitializeComponent();

		ProductTitle.Text = product.Title;
		Category.Text = Data.Categories[product.Category];
		Description.Text = product.Description;
		PrimaryPicture.Source = product.Url;
    }
}