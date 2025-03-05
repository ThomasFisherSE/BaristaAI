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
            _ = SubmitPrompt(PromptEntry.Text);
        }

        private async Task<bool> SubmitPrompt(string prompt)
        {
            // Disable the UI while waiting for the response
            BlockNewPrompts();

            var success = true;
            try
            {
                await _viewModel.GetChatResponse(prompt);
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
                AllowNewPrompts();
            }

            return success;
        }

        private void BlockNewPrompts()
        {
            SubmitButton.IsEnabled = false;
            SubmitButton.Text = "I'm thinking about it, please wait...";
            PromptEntry.IsEnabled = false;
        }

        private void AllowNewPrompts()
        {
            SubmitButton.Text = "Submit";
            SubmitButton.IsEnabled = true;
            PromptEntry.Text = string.Empty;
            PromptEntry.IsEnabled = true;
        }
    }
}
