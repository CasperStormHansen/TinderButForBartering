using Microsoft.AspNetCore.SignalR.Client;

namespace TinderButForBartering;

public partial class MatchPage : ContentPage
{
    private Match Match { get; set; }

    public MatchPage(Match match)
	{
		InitializeComponent();

        Match = match;

		Title = "Chat med " + Match.Name;

		ProductsView.ItemsSource = Match.ForeignProducts;
        ChatView.ItemsSource = Match.Messages;
    }

    private async void OnProduct_Clicked(object sender, EventArgs e)
    {
        ImageButton button = sender as ImageButton;
        Product product = button.BindingContext as Product;
        await Navigation.PushAsync(new ForeignProductsDetailsPage(product, false));
    }

    private async void OnSendButton_Clicked(object sender, EventArgs e) // TODO
	{
        string messageContent = myChatMessage.Text;
		myChatMessage.Text = string.Empty;
        sendButton.IsEnabled = false;

        bool success = false;
        while (!success)
        {
            (success, string errorMessage) = await Data.SendMessage(Match, messageContent);

            if (success)
            {
                //ChatView.ItemsSource = null;
                //ChatView.ItemsSource = Match.Messages; // does this refresh?
            }
            else
            {
                await DisplayAlert("Beskeden kunne ikke sendes", errorMessage, "OK");
            }
        }

        sendButton.IsEnabled = true;
    }
}