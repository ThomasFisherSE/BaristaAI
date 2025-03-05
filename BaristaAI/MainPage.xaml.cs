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

        private void OnSubmitClicked(object sender, EventArgs e)
        {
            _ = SubmitMessage(MessageEntry.Text);
        }

        private async Task<bool> SubmitMessage(string message)
        {
            // Disable the UI while waiting for the response
            BlockNewMessages();

            var success = true;
            try
            {
                await _viewModel.GetChatResponse(message);
            }
            catch (Exception exception)
            {
                success = false;
                await Console.Error.WriteLineAsync(
                    $"An unexpected error occurred while getting a chat response: {exception}");
            }
            finally
            {
                // Revert UI back to ready state
                AllowNewMessages();
            }

            return success;
        }

        private void BlockNewMessages()
        {
            SubmitButton.IsEnabled = false;
            SubmitButton.Text = "I'm thinking about it, please wait...";
            MessageEntry.IsEnabled = false;
        }

        private void AllowNewMessages()
        {
            SubmitButton.Text = "Submit";
            SubmitButton.IsEnabled = true;
            MessageEntry.Text = string.Empty;
            MessageEntry.IsEnabled = true;
        }
    }
}
