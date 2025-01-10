using Microsoft.Extensions.Configuration;
using Mscc.GenerativeAI;

namespace BaristaAI.Services
{
    internal class GeminiService : ILLMService
    {
        private const string UninitializedModelExceptionMsg = $"Must initialize model ({nameof(InitializeModel)}) before using it";
        private const string DefaultGenerativeModelType = Model.Gemini15Pro;

        private readonly GoogleAI _geminiClient;

        private GenerativeModel? _geminiModel;
        private ChatSession? _chatSession;

        public GeminiService()
        {
            _geminiClient = new GoogleAI(GetAPIKey());
        }

        public void InitializeModel(string? contextString)
        {
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

        private static string? GetAPIKey()
        {
            var builder = new ConfigurationBuilder().AddUserSecrets<GeminiService>();
            var configuration = builder.Build();
            return configuration["GeminiApiKey"];
        }
    }
}
