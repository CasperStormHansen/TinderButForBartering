using Microsoft.AspNetCore.SignalR.Client;

namespace TinderButForBartering;

public partial class MatchPage : ContentPage
{
	public MatchPage(Match match)
	{
		InitializeComponent();

		Title = "Chat med " + match.Name;

		ProductsView.ItemsSource = match.ForeignProducts;

        Backend.ComHubConnection.On<string>("MessageReceived", (message) =>
		{
            chatMessages.Dispatcher.Dispatch(() => {
                chatMessages.Text += $"{Environment.NewLine}{message}";
            });
        });

		Task.Run(() =>
		{
			Dispatcher.Dispatch(async () =>
			await Backend.ComHubConnection.StartAsync());
		});
	}

    private async void OnProduct_Clicked(object sender, EventArgs e)
    {
        ImageButton button = sender as ImageButton;
        Product product = button.BindingContext as Product;
        await Navigation.PushAsync(new ForeignProductsDetailsPage(product, false));
    }

    private async void OnSendButton_Clicked(object sender, EventArgs e)
	{
		await Backend.ComHubConnection.InvokeCoreAsync("SendMessage", args: new[]
		{ myChatMessage.Text });

		myChatMessage.Text = String.Empty;
	}
}