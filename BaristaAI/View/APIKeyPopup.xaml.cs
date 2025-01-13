using CommunityToolkit.Maui.Views;

namespace BaristaAI.View;

public partial class APIKeyPopup : Popup
{
	public APIKeyPopup()
	{
		InitializeComponent();
	}

	private void OnSubmitClicked(object sender, EventArgs e)
	{
		var apiKey = APIKeyEntry.Text;
		Close(apiKey);
	}
}