using static TinderButForBartering.BackendConnection;
using static TinderButForBartering.Pictures;
using static TinderButForBartering.Data;

namespace TinderButForBartering;

public partial class UserProductDetailsPage : ContentPage
{
    public UserProductDetailsPage()
    {
        InitializeComponent();
    }

    public UserProductDetailsPage(Product product)
    {
        InitializeComponent();
        Titel.Text = product.Title;
        Description.Text = product.Description;
        Switch.IsToggled = product.RequiresSomethingInReturn;
        PrimaryPictureData = product.PrimaryPictureData;
        MemoryStream stream = new MemoryStream(PrimaryPictureData); // dispose somewhere
        PrimaryPicture.Source = ImageSource.FromStream(() => stream); // rename "stream"
    }

    async void OnAddProduct_Clicked(object sender, EventArgs e)
    {
        ProductWithoutId product = new (Titel.Text, Description.Text, Switch.IsToggled, PrimaryPictureData);
        string productString = await PostProduct(product);
        AddNewOwnProduct(productString);
        await Navigation.PopAsync();
    }

    async void OnCancel_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    byte[] PrimaryPictureData { get; set; }

    async void OnTakePicture_Clicked(object sender, EventArgs e)
    {
#nullable enable
        MemoryStream? imageStream = await GetPhoto(true); // where does imageStream.Dispose() go? At the close of the page?
#nullable disable
        if (imageStream != null) 
        {
            PrimaryPicture.Source = ImageSource.FromStream(() => imageStream);
            PrimaryPictureData = imageStream.ToArray();
        }
    }

    async void OnSelectPicture_Clicked(object sender, EventArgs e)
    {
#nullable enable
        MemoryStream? imageStream = await GetPhoto(false); // where does imageStream.Dispose() go? At the close of the page?
#nullable disable
        if (imageStream != null)
        {
            PrimaryPicture.Source = ImageSource.FromStream(() => imageStream);
            PrimaryPictureData = imageStream.ToArray();
        }
    }
}