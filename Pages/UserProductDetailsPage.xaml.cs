using static TinderButForBartering.Pictures;
using static TinderButForBartering.Data;

namespace TinderButForBartering;

public partial class UserProductDetailsPage : ContentPage
{
#nullable enable
    Product? Product { get; set; }
    MemoryStream? PrimaryPictureStream { get; set; }
    byte[]? PrimaryPictureData { get; set; }
#nullable disable

    // When loaded as "add new product page"
    public UserProductDetailsPage()
    {
        InitializeComponent();

        ChangeProductButton.IsVisible = false;
        DeleteProductButton.IsVisible = false;
    }

    // When loaded as "modify product page"
    public UserProductDetailsPage(Product product)
    {
        InitializeComponent();

        AddNewProductButton.IsVisible = false;

        Product = product;

        Title.Text = Product.Title;
        Description.Text = Product.Description;
        Switch.IsToggled = Product.RequiresSomethingInReturn;
        PrimaryPicture.Source = Product.Url;
        //PrimaryPictureData = Product.PrimaryPictureData;
        //PrimaryPictureStream = new(PrimaryPictureData);
        //PrimaryPicture.Source = ImageSource.FromStream(() => PrimaryPictureStream);
    }

    async void OnAddProduct_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Title.Text) || PrimaryPictureData == null)
        {
            await Application.Current.MainPage.DisplayAlert("Det er nødvendigt med en titel og mindst et billede", "", "OK");
            return;
        }

        BusyIndicator.On();
        ProductWithoutId productWithoutId = new (Title.Text, Description.Text?? "", Switch.IsToggled, PrimaryPictureData);
        (bool wasSuccessful, string errorInfo) = await AddNewOwnProduct(productWithoutId);
        BusyIndicator.Off();

        if (wasSuccessful)
        {
            await Navigation.PopAsync();
            return;
        }

        await Application.Current.MainPage.DisplayAlert("Noget gik galt, så din vare er ikke blevet tilføjet", errorInfo, "OK");
    }

    async void OnChangeProduct_Clicked(object sender, EventArgs e)
    {
        BusyIndicator.On();
        Product changedProduct = new(Title.Text, Description.Text?? "", Switch.IsToggled, PrimaryPictureData, Product.Id);
        (bool wasSuccessful, string errorInfo) = await ChangeOwnProduct(changedProduct);
        BusyIndicator.Off();

        if (wasSuccessful)
        {
            await Navigation.PopAsync();
            return;
        }

        await Application.Current.MainPage.DisplayAlert("Noget gik galt, så din vare er ikke blevet opdateret", errorInfo, "OK");
    }

    async void OnCancel_Clicked(object sender, EventArgs e)
    {
        bool changeHasBeenMade = 
            (Product == null 
                && (!string.IsNullOrWhiteSpace(Title.Text) || !string.IsNullOrWhiteSpace(Description.Text) || PrimaryPictureData != null)
            )
            || 
            (Product != null
                && (Title.Text != Product.Title || Description.Text != Product.Description || Switch.IsToggled != Product.RequiresSomethingInReturn || PrimaryPictureData != null)
            );
        if (changeHasBeenMade)
        {
            string userDecision = await Application.Current.MainPage.DisplayActionSheet("Fortryd ændringer?", "Nej", "Ja");
            if (userDecision == "Nej")
            {
                return;
            }
        }

        await Navigation.PopAsync();
    }

    async void OnDeleteProduct_Clicked(object sender, EventArgs e)
    {
        string userDecision = await Application.Current.MainPage.DisplayActionSheet("Slet vare?", "Fortryd", "OK");
        if (userDecision == "Fortryd")
        {
            return;
        }

        BusyIndicator.On();
        (bool wasSuccessful, string errorInfo) = await DeleteOwnProduct(Product);
        BusyIndicator.Off();

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