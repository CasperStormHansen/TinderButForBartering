namespace TinderButForBartering;

public partial class MyMatchesPage : ContentPage
{
	public MyMatchesPage()
	{
		InitializeComponent();
	}

    private async void OnMatch_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MatchPage());
    }
}