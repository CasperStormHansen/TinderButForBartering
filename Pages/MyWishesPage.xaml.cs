using System.Collections.ObjectModel;

namespace TinderButForBartering;

public partial class MyWishesPage : ContentPage
{
	public ObservableCollection<Wish> Wishes { get; private set; } = new();

    public MyWishesPage()
	{
		InitializeComponent();

		for (int i = 0; i < Data.Categories.Count; i++) // int -> byte?
		{
			Wishes.Add(new Wish(Data.CurrentUser.Wishlist, i, Data.Categories[i]));
		}

		MyWishesView.ItemsSource = Wishes;
		//MyWishesView.ItemsSource = Data.Categories;
    }

    async void OnSave_Clicked(object sender, EventArgs e)
    {
        //byte[] newWishlist = Wishes.Select(w => w.On ? (byte)1 : (byte)0).ToArray();
        byte[] newWishlist = Wishes
            .Select(w => w.On)
            .Select((v, i) => new { v, i })
            .Where(x => x.v)
            .Select(x => (byte)x.i)
            .ToArray();

        if (newWishlist.SequenceEqual(Data.CurrentUser.Wishlist))
		{
            await Navigation.PopAsync();
            return;
        }

        BusyIndicator.On();
        Data.CurrentUser.Wishlist = newWishlist; // should that happen here? What if backend cannot be reached?
        bool success = await Data.OnWishesUpdate(Data.CurrentUser);
        BusyIndicator.Off();

        if (success)
        {
            await Navigation.PopAsync();
            return;
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
	public string Cat { get; set; }

	public Wish(byte[] wishlist, int index, string cat)
	{
		On = (wishlist.Contains((byte)index));
		Cat = cat;
	}
}