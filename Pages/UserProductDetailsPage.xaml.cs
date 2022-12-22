using static TinderButForBartering.Pictures;
using static TinderButForBartering.Data;
using CommunityToolkit.Maui.Views;

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
        if (string.IsNullOrWhiteSpace(Title.Text) || PrimaryPictureData == null)
        {
            await Application.Current.MainPage.DisplayAlert("Det er nødvendigt med en titel og mindst et billede", "", "OK");
            return;
        }

        SpinnerPopup busyIndicator = new(); this.ShowPopup(busyIndicator);
            ProductWithoutId productWithoutId = new (Title.Text, Description.Text?? "", Switch.IsToggled, PrimaryPictureData);
            (bool wasSuccessful, string errorInfo) = await AddNewOwnProduct(productWithoutId);
        busyIndicator.Close();

        if (wasSuccessful)
        {
            await Navigation.PopAsync();
            return;
        }

        await Application.Current.MainPage.DisplayAlert("Noget gik galt, så din vare er ikke blevet tilføjet", errorInfo, "OK");
    }

    async void OnChangeProduct_Clicked(object sender, EventArgs e)
    {
        SpinnerPopup busyIndicator = new(); this.ShowPopup(busyIndicator);
            Product changedProduct = new(Title.Text, Description.Text?? "", Switch.IsToggled, PrimaryPictureData, Product.Id);
            (bool wasSuccessful, string errorInfo) = await ChangeOwnProduct(changedProduct);
        busyIndicator.Close();

        if (wasSuccessful)
        {
            await Navigation.PopAsync();
            return;
        }

        await Application.Current.MainPage.DisplayAlert("Noget gik galt, så din vare er ikke blevet opdateret", errorInfo, "OK");
    }

    async void OnCancel_Clicked(object sender, EventArgs e)
    {
        // perhaps a warning dialog here, which should also appear on back button click
        await Navigation.PopAsync();
    }

    async void OnDeleteProduct_Clicked(object sender, EventArgs e)
    {
        string userDecision = await Application.Current.MainPage.DisplayActionSheet("Slet vare?", "Fortryd", "OK");
        if (userDecision == "Fortryd")
        {
            return;
        }

        SpinnerPopup busyIndicator = new(); this.ShowPopup(busyIndicator);
            (bool wasSuccessful, string errorInfo) = await DeleteOwnProduct(Product);
        busyIndicator.Close();

        if (wasSuccessful)
        {
            await Navigation.PopAsync();
            return;
        }

        await Application.Current.MainPage.DisplayAlert("Noget gik galt, så din vare er ikke blevet slettet", errorInfo, "OK");
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
}