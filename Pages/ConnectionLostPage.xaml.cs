namespace TinderButForBartering;

public partial class ConnectionLostPage : ContentPage
{
	public ConnectionLostPage()
	{
		InitializeComponent();
        Connectivity.ConnectivityChanged += ConnectivityChanged;
        AttemptToReconnect();
    }

	private async Task AttemptToReconnect()
	{
        ReestablishButton.IsVisible = false;
		ReestablishingInfo.IsVisible = true;

        bool success = await Backend.Reconnect();

		if (success)
		{
            Connectivity.ConnectivityChanged -= ConnectivityChanged; 
			await Navigation.PopAsync();
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