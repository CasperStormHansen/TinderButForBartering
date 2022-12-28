using CommunityToolkit.Maui.Views;

namespace TinderButForBartering;

public partial class BusyIndicator : Popup
{
	#nullable enable
	static BusyIndicator? currentSpinnerPopup;
	#nullable disable

	public BusyIndicator()
	{
		InitializeComponent();
	}

	public static void On()
	{
        Page currentPage = Application.Current.MainPage;
        currentSpinnerPopup = new BusyIndicator();
        currentPage.ShowPopup(currentSpinnerPopup);
    }

    public static void Off()
    {
        currentSpinnerPopup.Close();
    }
}