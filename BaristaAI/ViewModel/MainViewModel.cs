using BaristaAI.Services.LLM;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BaristaAI.ViewModel
{
    public partial class MainViewModel : INotifyPropertyChanged
    {
        private readonly ILLMService _llmService;

        private string _responseText = string.Empty;
        private string _initialMessageText = "Hi, I'm your personal AI barista assistant! How can I help?";
        
        private const string DefaultBaristaAssistantContext = 
            """
            You are an expert barista who assists home brewers in perfecting their coffee.
            
            Aim to quickly steer conversations towards providing assistance with coffee brewing.
            
            Be informative about coffee and coffee brewing whilst remaining concise.
            
            Avoid asking too many follow up questions at once so that the conversation flows naturally.
            
            Start out as if you are speaking to a novice coffee hobbyist, but adjust as the conversation progresses if
            you think the person you are talking to has more advanced knowledge.
            
            Provide responses as cleanly formatted markdown, using headers where appropriate and bullet pointed / 
            numbered lists.
            """;
        
        public event PropertyChangedEventHandler? PropertyChanged;
        
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
            _ = InitializeBaristaChatSession();
        }
        
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task SubmitMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                ResponseText = "Please type your message into the text box above before hitting submit.";
            else
            {
                var response = await _llmService.GetChatResponse(message);
                
                ResponseText = string.IsNullOrEmpty(response.Text)
                    ? "Sorry, something went wrong. Please try again."
                    : response.Text;
            }
        }

        private async Task InitializeBaristaChatSession()
        {
            await _llmService.InitializeModel(contextString: DefaultBaristaAssistantContext);
            _llmService.StartNewChatSession(InitialMessageText);
        }
    }
}
