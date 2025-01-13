using Mscc.GenerativeAI;

namespace BaristaAI.Services.LLM
{
    internal class GeminiService : ILLMService
    {
        private const string UninitializedModelExceptionMsg = $"Must initialize model ({nameof(InitializeModel)}) before using it";
        private const string DefaultGenerativeModelType = Model.Gemini15Pro;
        private const string APIKeyName = "gemini-api-key";

        private readonly IAPIKeyService _apiKeyService;

        private GoogleAI? _geminiClient;
        private GenerativeModel? _geminiModel;
        private ChatSession? _chatSession;

        public GeminiService(IAPIKeyService apiKeyService)
        {
            _apiKeyService = apiKeyService;
        }

        public async Task InitializeModel(string? contextString)
        {
            _geminiClient ??= await InitializeGeminiClient();

            Content? systemInstructionContent = contextString != null ? new Content(contextString) : null;
            _geminiModel = _geminiClient.GenerativeModel(DefaultGenerativeModelType, systemInstruction: systemInstructionContent);
        }

        public void BeginNewChat(string? initialModelMessage = null)
        {
            if (_geminiModel == null)
                throw new InvalidOperationException();

            _chatSession = _geminiModel.StartChat();

            // Prepare initial message history
            List<ContentResponse> history = [];
            if (initialModelMessage != null)
                history.Add(new ContentResponse(initialModelMessage, "model"));

            _geminiModel.StartChat(history);
        }

        public async Task<string?> GetChatResponse(string message)
        {
            if (_chatSession == null)
                throw new InvalidOperationException(UninitializedModelExceptionMsg);

            var response = await _chatSession.SendMessage(message);
            return response.Text;
        }

        public async Task<string?> GetTextResponse(string prompt)
        {
            if (_geminiModel == null)
                throw new InvalidOperationException(UninitializedModelExceptionMsg);

            var response = await _geminiModel.GenerateContent(prompt);
            return response.Text;
        }

        private async Task<GoogleAI> InitializeGeminiClient()
        {
            var apiKey = await _apiKeyService.RequestAPIKey(APIKeyName);
            return new GoogleAI(apiKey);
        }
    }
}
