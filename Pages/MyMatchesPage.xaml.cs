namespace TinderButForBartering;

public partial class MyMatchesPage : ContentPage
{
	public MyMatchesPage()
	{
		InitializeComponent();

        MyMatchesView.ItemsSource = Data.Matches; // Collections in here are arrays and not observablecollections, so updates will not work correctly
    }

    private async void OnMatch_Clicked(object sender, EventArgs e) // to be modified
    {
        Button button = sender as Button;
        Match match = button.BindingContext as Match;
        await Navigation.PushAsync(new MatchPage(match));
    }
}