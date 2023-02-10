namespace TinderButForBartering;

public partial class ConnectionLostPage : ContentPage
{
	public ConnectionLostPage()
	{
		InitializeComponent();
	}

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        Connectivity.ConnectivityChanged += ConnectivityChanged;
        AttemptToReconnect();
    }

    private async Task AttemptToReconnect()
	{
        ReestablishButton.IsVisible = false;
		ReestablishingInfo.IsVisible = true;

        bool success = await Backend.Reconnect(); // This takes more than a minute to timeout. 7 secs would be better.

		if (success)
		{
            Connectivity.ConnectivityChanged -= ConnectivityChanged; 
			await Navigation.PopModalAsync();
        }
		else
		{
			ReestablishButton.IsVisible = true;
			ReestablishingInfo.IsVisible = false;
		}
	}

    private async void ReestablishButton_Clicked(object sender, EventArgs e)
	{
		await AttemptToReconnect();
    }

    private async void ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
        await AttemptToReconnect();
    }
}