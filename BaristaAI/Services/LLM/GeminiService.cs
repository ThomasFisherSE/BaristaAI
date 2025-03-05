using Mscc.GenerativeAI;

namespace BaristaAI.Services.LLM
{
    internal class GeminiService(IAPIKeyService apiKeyService) : ILLMService
    {
        private const string UninitializedModelExceptionMsg = $"Must initialize model ({nameof(InitializeModelWithCurrentData)}) before using it";
        private const string DefaultGenerativeModelType = Model.Gemini15Pro;
        private const string APIKeyName = "gemini-api-key";

        private const string InvalidAPIKeyExceptionContent = "API key not valid";

        private GoogleAI? _geminiClient;
        private GenerativeModel? _geminiModel;
        private ChatSession? _chatSession;

        private string? _contextString;

        public async Task InitializeModel(string? contextString = null)
        {
            _contextString = contextString;
            await InitializeModelWithCurrentData();
        }

        private async Task InitializeModelWithCurrentData()
        {
            _geminiClient = await InitializeGeminiClient();

            var systemInstructionContent = _contextString != null ? new Content(_contextString) : null;
            _geminiModel = _geminiClient.GenerativeModel(model: DefaultGenerativeModelType, 
                systemInstruction: systemInstructionContent);
        }

        public void StartNewChatSession(string? initialModelMessage = null)
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
            return await AttemptContentGeneration(message, GenerateChatMessageResponse);
        }

        public async Task<string?> GetTextContentFromPrompt(string prompt)
        {
            return await AttemptContentGeneration(prompt, GenerateTextFromPrompt);
        }
        
        private async Task<string?> AttemptContentGeneration(string prompt, Func<string, Task<string?>> contentGenerationFunc)
        {
            string? result = null;
            try
            {
                result = await contentGenerationFunc(prompt);
            }
            catch (HttpRequestException httpRequestException) 
                when (httpRequestException.Message.Contains(InvalidAPIKeyExceptionContent))
            {
                if (httpRequestException.Message.Contains(InvalidAPIKeyExceptionContent))
                {
                    await AttemptToFixAPIKey();
                    result = await GetTextContentFromPrompt(prompt);
                }
            }
            catch (Exception exception)
            {
                await Console.Error.WriteLineAsync($"Unexpected error occurred during content generation: {exception}");
            }
            
            return result;
        }
        
        private async Task<string?> GenerateChatMessageResponse(string message)
        {
            if (_chatSession == null)
                throw new InvalidOperationException(UninitializedModelExceptionMsg);
            
            var response = await _chatSession.SendMessage(message);
            return response.Text;
        }

        private async Task<string?> GenerateTextFromPrompt(string prompt)
        {
            if (_geminiModel == null)
                throw new InvalidOperationException(UninitializedModelExceptionMsg);
            
            var response = await _geminiModel.GenerateContent(prompt);
            return response.Text;
        }
        
        private async Task AttemptToFixAPIKey()
        {
            // Re-acquire API key
            apiKeyService.RemoveCurrentAPIKey();
            await InitializeModelWithCurrentData();
        }

        private async Task<GoogleAI> InitializeGeminiClient()
        {
            var apiKey = await apiKeyService.RequestAPIKey(APIKeyName);
            return new GoogleAI(apiKey);
        }
    }
}
