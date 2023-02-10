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
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        AttemptToReconnect();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
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