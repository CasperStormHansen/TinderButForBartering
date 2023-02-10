using System.Collections.ObjectModel;

namespace TinderButForBartering;

public partial class MyWishesPage : ContentPage
{
	public ObservableCollection<Wish> WishesForBinding { get; set; } = new();

    public MyWishesPage()
	{
		InitializeComponent();

		for (byte i = 0; i < Data.Categories.Length; i++) WishesForBinding.Add(new Wish(i));

		MyWishesView.ItemsSource = WishesForBinding;
    }

    async void OnSave_Clicked(object sender, EventArgs e)
    {
        byte[] newWishlist = WishesForBinding
            .Select(wish => wish.On)
            .Select((wishOn, index) => new { wishOn, index })
            .Where(item => item.wishOn)
            .Select(item => (byte)item.index)
            .ToArray();

        if (newWishlist.SequenceEqual(Data.CurrentUser.Wishlist))
		{
            await Navigation.PopAsync();
            return;
        }

        BusyIndicator.On();
        (bool success, string errorMessage) = await Data.OnWishesUpdate(newWishlist);
        BusyIndicator.Off();

        if (success)
        {
            await Navigation.PopAsync();
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("Der kunne ikke opnås kontakt til serveren", errorMessage, "OK");
        }
    }

    async void OnCancel_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}

public class Wish
{
	public bool On { get; set; }
	public string Category { get; set; }

	public Wish(byte i)
	{
		On = (Data.CurrentUser.Wishlist.Contains(i));
		Category = Data.Categories[i];
	}
}