using static TinderButForBartering.Pictures;
using static TinderButForBartering.Data;

namespace TinderButForBartering;

public partial class UserProductDetailsPage : ContentPage
{
    #nullable enable
    Product? Product { get; set; }
    #nullable disable

    public UserProductDetailsPage()
    {
        InitializeComponent();

        ChangeProductButton.IsVisible = false;
        DeleteProductButton.IsVisible = false;
    }

    public UserProductDetailsPage(Product product)
    {
        InitializeComponent();

        AddNewProductButton.IsVisible = false;

        Product = product;

        Titel.Text = Product.Title;
        Description.Text = Product.Description;
        Switch.IsToggled = Product.RequiresSomethingInReturn;
        PrimaryPictureData = Product.PrimaryPictureData;
        MemoryStream stream = new (PrimaryPictureData); // dispose somewhere
        PrimaryPicture.Source = ImageSource.FromStream(() => stream); // rename "stream"
    }

    async void OnAddProduct_Clicked(object sender, EventArgs e)
    {
        ProductWithoutId productWithoutId = new (Titel.Text, Description.Text, Switch.IsToggled, PrimaryPictureData);
        AddNewOwnProduct(productWithoutId); // successindicator should come back and be acted on
        await Navigation.PopAsync();
    }

    async void OnChangeProduct_Clicked(object sender, EventArgs e)
    {
        Product changedProduct = new(Titel.Text, Description.Text, Switch.IsToggled, PrimaryPictureData, Product.Id);
        ChangeOwnProduct(changedProduct);
        await Navigation.PopAsync();
    }

    async void OnCancel_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    async void OnDeleteProduct_Clicked(object sender, EventArgs e)
    {
        DeleteOwnProduct(Product);
        await Navigation.PopAsync();
    }

    byte[] PrimaryPictureData { get; set; }

    async void OnGetPicture_Clicked(object sender, EventArgs e)
    {
        Button button = sender as Button;
        bool useCamera = (button == CameraButton);

        #nullable enable
        MemoryStream? imageStream = await GetPhoto(useCamera); // where does imageStream.Dispose() go? At the close of the page?
        #nullable disable

        if (imageStream != null) 
        {
            PrimaryPicture.Source = ImageSource.FromStream(() => imageStream);
            PrimaryPictureData = imageStream.ToArray();
        }
    }
}