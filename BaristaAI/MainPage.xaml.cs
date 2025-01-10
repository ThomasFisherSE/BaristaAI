using BaristaAI.ViewModel;

namespace BaristaAI
{
    public partial class MainPage : ContentPage
    {
        private readonly MainViewModel _viewModel;

        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        private async void OnSubmitClicked(object sender, EventArgs e)
        {
            string prompt = PromptEntry.Text;

            // Disable the UI while waiting for the response
            SubmitButton.IsEnabled = false;
            SubmitButton.Text = "I'm thinking about it, please wait...";
            PromptEntry.IsEnabled = false;
            await _viewModel.GetChatResponse(prompt);

            // Revert UI back to ready state
            SubmitButton.Text = "Submit";
            SubmitButton.IsEnabled = true;
            PromptEntry.Text = string.Empty;
            PromptEntry.IsEnabled = true;
        }
    }
}
