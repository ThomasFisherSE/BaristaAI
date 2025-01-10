using BaristaAI.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BaristaAI.ViewModel
{
    public partial class MainViewModel : INotifyPropertyChanged
    {
        private readonly ILLMService _llmService;

        private string _responseText = string.Empty;
        private string _initialMessageText = "Hi, I'm your personal AI barista assistant! Let me know any issues with your coffee and I'll try and help you fix it.";

        public string ResponseText
        {
            get => _responseText;
            set
            {
                _responseText = value;
                OnPropertyChanged();
            }
        }

        public string InitialMessageText
        {
            get => _initialMessageText;
            set
            {
                _initialMessageText = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel(ILLMService llmService)
        {
            _llmService = llmService;
            InitializeBaristaChatSession();
        }

        public async Task GetChatResponse(string message)
        {
            if (string.IsNullOrEmpty(message))
                ResponseText = "Please type your message into the text box above before hitting submit.";
            else
                ResponseText = await _llmService.GetChatResponse(message) ?? "Sorry, something went wrong. Please try again.";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void InitializeBaristaChatSession()
        {
            string baristaContext = "You are an expert barista who likes to help home brewers to perfect their coffee. " + 
                "From now on, you only answer questions in ways that relate to coffee brewing, otherwise you ask if there's anything coffee brewing related that you can help with.";
            _llmService.InitializeModel(baristaContext);
            _llmService.BeginNewChat(InitialMessageText);
        }
    }
}
