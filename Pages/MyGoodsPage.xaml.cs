namespace TinderButForBartering;

public partial class MyGoodsPage : ContentPage
{
    public MyGoodsPage()
    {
        InitializeComponent();
        RenderPage();
    }

    public void RenderPage()
    {
        flexLayout.Children.Clear();

        double totalWidth = Application.Current.MainPage.Width;
        double marginLength = 0.08 * totalWidth;
        double itemLength = (totalWidth - 3 * marginLength) / 2 ;

        foreach (Product product in Data.OwnProducts) AddPicture(product, itemLength);
        AddButton(itemLength);
        if (Data.OwnProducts.Count % 2 == 0) AddInvisibleItem(itemLength);

        int numberOfRows = Data.OwnProducts.Count / 2 + 1;
        double totalHeigth = numberOfRows * itemLength + (numberOfRows + 1) * marginLength;
        flexLayout.HeightRequest = totalHeigth;
        flexLayout.VerticalOptions = LayoutOptions.Start;
    }

    private void AddPicture(Product product, double itemLength)
    {
        Grid grid = new()
        {
            HeightRequest = itemLength,
            WidthRequest = itemLength
        };

        Frame frame = new()
        {
            HeightRequest = itemLength,
            WidthRequest = itemLength,
            CornerRadius = 13,
            IsClippedToBounds = true,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Content = new Image
            {
                Source = product.Url,
                Aspect = Aspect.AspectFill,
                //Margin = new Thickness(-20),
                HeightRequest = itemLength,
                WidthRequest = itemLength
            }
        };

        ImageButton imageButton = new()
        {
            WidthRequest = itemLength,
            HeightRequest = itemLength,
            BackgroundColor = Colors.Transparent,
            BindingContext = product
        };
        imageButton.Clicked += OnUserProductDetails_Clicked;

        grid.Children.Add(frame);
        grid.Children.Add(imageButton);

        flexLayout.Children.Add(grid);
    }

    private void AddButton(double itemLength)
    {
        Button button = new()
        {
            Text = "+",
            FontSize = 70,
            TextColor = Colors.Black,
            BackgroundColor = new Microsoft.Maui.Graphics.Color(171, 177, 97), // Ref to ressource dictionary would be better
            WidthRequest = itemLength,
            HeightRequest = itemLength,
            CornerRadius = 13
        };
        button.Clicked += OnNewProduct_Clicked;

        flexLayout.Children.Add(button);
    }

    private void AddInvisibleItem(double itemLength)
    {
        BoxView transparentBox = new()
        {
            Color = Colors.Transparent,
            WidthRequest = itemLength,
            HeightRequest = itemLength
        };

        flexLayout.Children.Add(transparentBox);
    }

    private async void OnUserProductDetails_Clicked(object sender, EventArgs e)
    {
        ImageButton button = sender as ImageButton;
        Product product = button.BindingContext as Product;
        await Navigation.PushAsync(new UserProductDetailsPage(this, product));
    }

    private async void OnNewProduct_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new UserProductDetailsPage(this));
    }
}