using TinderButForBartering;

namespace TinderButForBartering;

public partial class MyMatchesPage : ContentPage
{
	public MyMatchesPage()
	{
		InitializeComponent();

        MyMatchesView.ItemsSource = Data.Matches;
    }

    private async void OnMatch_Clicked(object sender, EventArgs e)
    {
        Button button = sender as Button;
        Match match = button.BindingContext as Match;
        await Navigation.PushAsync(new MatchPage(match));
    }

    private async void OnProduct_Clicked(object sender, EventArgs e)
    {
        ImageButton button = sender as ImageButton;
        Product product = button.BindingContext as Product;
        await Navigation.PushAsync(new ForeignProductsDetailsPage(product, false));
    }
}