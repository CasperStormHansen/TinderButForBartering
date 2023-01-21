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

        CategoryPicker.ItemsSource = Data.Categories;
    }

    // When loaded as "modify product page"
    public UserProductDetailsPage(Product product)
    {
        InitializeComponent();

        AddNewProductButton.IsVisible = false;

        Product = product;

        ProductTitle.Text = Product.Title;
        Description.Text = Product.Description;
        Switch.IsToggled = Product.RequiresSomethingInReturn;
        PrimaryPicture.Source = Product.Url;

        CategoryPicker.ItemsSource = Data.Categories;
        CategoryPicker.SelectedIndex = Product.Category;
    }

    async void OnAddProduct_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ProductTitle.Text) || CategoryPicker.SelectedIndex == -1 || PrimaryPictureData == null)
        {
            await Application.Current.MainPage.DisplayAlert("Det er nødvendigt med en titel, en kategori og mindst ét billede", "", "OK");
            return;
        }

        BusyIndicator.On();
        ProductWithoutId productWithoutId = new (ProductTitle.Text, (byte)CategoryPicker.SelectedIndex, Description.Text?? "", Switch.IsToggled, PrimaryPictureData);
        (bool wasSuccessful, string errorInfo) = await Data.AddNewOwnProduct(productWithoutId);
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
        Product changedProduct = new(ProductTitle.Text, (byte)CategoryPicker.SelectedIndex, Description.Text?? "", Switch.IsToggled, PrimaryPictureData, Product.Id);
        (bool wasSuccessful, string errorInfo) = await Data.ChangeOwnProduct(changedProduct);
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
                && (!string.IsNullOrWhiteSpace(ProductTitle.Text) || !string.IsNullOrWhiteSpace(Description.Text) || PrimaryPictureData != null)
            )
            || 
            (Product != null
                && (ProductTitle.Text != Product.Title || (byte)CategoryPicker.SelectedIndex != Product.Category || Description.Text != Product.Description || Switch.IsToggled != Product.RequiresSomethingInReturn || PrimaryPictureData != null)
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
        (bool wasSuccessful, string errorInfo) = await Data.DeleteOwnProduct(Product);
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

        PrimaryPictureStream = await Pictures.GetPhoto(useCamera);

        if (PrimaryPictureStream != null)
        {
            PrimaryPicture.Source = ImageSource.FromStream(() => PrimaryPictureStream);
            PrimaryPictureData = PrimaryPictureStream.ToArray();
        }
    }
}