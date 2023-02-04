using Microsoft.AspNetCore.SignalR.Client;

namespace TinderButForBartering;

public partial class MatchPage : ContentPage
{
	private readonly HubConnection _connection;

	public MatchPage(Match match)
	{
		InitializeComponent();

		Title = "Chat med " + match.Name;

		ProductsView.ItemsSource = match.ForeignProducts;

        _connection = new HubConnectionBuilder()
			.WithUrl("http://10.0.2.2:5045/chat")
			.Build();

		_connection.On<string>("MessageReceived", (message) =>
		{
            chatMessages.Dispatcher.Dispatch(() => {
                chatMessages.Text += $"{Environment.NewLine}{message}";
            });
        });

		Task.Run(() =>
		{
			Dispatcher.Dispatch(async () =>
			await _connection.StartAsync());
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
		await _connection.InvokeCoreAsync("SendMessage", args: new[]
		{ myChatMessage.Text });

		myChatMessage.Text = String.Empty;
	}
}