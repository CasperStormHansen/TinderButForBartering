using static TinderButForBartering.BackendConnection;
using static TinderButForBartering.Pictures;

namespace TinderButForBartering;

public partial class UserProductDetailsPage : ContentPage
{
    public UserProductDetailsPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Update();
    }

    async Task Update()
    {
        ProductDetailsLabel.Text = await GetProducts();
    }

    async void OnAddProduct_Clicked(object sender, EventArgs e)
    {
        Product product = new(Titel.Text, Description.Text, Switch.IsToggled, PrimaryPictureData);
        await PostProduct(product);
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