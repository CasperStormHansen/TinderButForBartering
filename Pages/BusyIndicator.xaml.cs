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

	/// <summary>
	/// Turns on a busy indicator that prevents user interaction.
	/// </summary>
	public static void On()
	{
        Page currentPage = Application.Current.MainPage;
        currentSpinnerPopup = new BusyIndicator();
        currentPage.ShowPopup(currentSpinnerPopup);
    }

    /// <summary>
    /// Turns off a busy indicator that prevents user interaction.
    /// </summary>
    public static void Off()
    {
        currentSpinnerPopup.Close();
    }
}