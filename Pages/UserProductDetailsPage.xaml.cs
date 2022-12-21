using static TinderButForBartering.Pictures;
using static TinderButForBartering.Data;

namespace TinderButForBartering;

public partial class UserProductDetailsPage : ContentPage
{
#nullable enable
    Product? Product { get; set; }
#nullable disable

    MemoryStream PrimaryPictureStream { get; set; }
    byte[] PrimaryPictureData { get; set; }

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

        Title.Text = Product.Title;
        Description.Text = Product.Description;
        Switch.IsToggled = Product.RequiresSomethingInReturn;
        PrimaryPictureData = Product.PrimaryPictureData;
        PrimaryPictureStream = new(PrimaryPictureData);
        PrimaryPicture.Source = ImageSource.FromStream(() => PrimaryPictureStream);
    }

    async void OnAddProduct_Clicked(object sender, EventArgs e)
    {
        if (String.IsNullOrWhiteSpace(Title.Text) || PrimaryPictureData == null)
        {
            await Application.Current.MainPage.DisplayAlert("Det er nødvendigt med en titel og mindst et billede", "", "OK");
            return;
        }

        ProductWithoutId productWithoutId = new(Title.Text, Description.Text?? "", Switch.IsToggled, PrimaryPictureData);
        AddNewOwnProduct(productWithoutId); // successindicator should come back and be acted on
        await Navigation.PopAsync();
    }

    async void OnChangeProduct_Clicked(object sender, EventArgs e)
    {
        Product changedProduct = new(Title.Text, Description.Text?? "", Switch.IsToggled, PrimaryPictureData, Product.Id);
        ChangeOwnProduct(changedProduct);
        await Navigation.PopAsync();
    }

    async void OnCancel_Clicked(object sender, EventArgs e)
    {
        // perhaps a warning dialog here
        await Navigation.PopAsync();
    }

    async void OnDeleteProduct_Clicked(object sender, EventArgs e)
    {
        string userDecision = await Application.Current.MainPage.DisplayActionSheet("Slet vare?", "Fortryd", "OK");
        if (userDecision == "OK")
        {
            DeleteOwnProduct(Product);
            await Navigation.PopAsync();
        }
    }

    async void OnGetPicture_Clicked(object sender, EventArgs e)
    {
        Button button = sender as Button;
        bool useCamera = (button == CameraButton);

        PrimaryPictureStream = await GetPhoto(useCamera);

        if (PrimaryPictureStream != null)
        {
            PrimaryPicture.Source = ImageSource.FromStream(() => PrimaryPictureStream);
            PrimaryPictureData = PrimaryPictureStream.ToArray();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (PrimaryPictureStream != null)
        {
            PrimaryPictureStream.Dispose();
        }
    }
}