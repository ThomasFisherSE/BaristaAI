using Mscc.GenerativeAI;
using GenerateTextResponse = BaristaAI.Model.GenerateTextResponse;

namespace BaristaAI.Services.LLM
{
    internal class GeminiService(IAPIKeyService apiKeyService) : ILLMService
    {
        private const string UninitializedModelExceptionMsg = $"Must initialize model ({nameof(InitializeModelWithCurrentData)}) before using it";
        private const string DefaultGenerativeModelType = Mscc.GenerativeAI.Model.Gemini15Pro;
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

        public async Task<GenerateTextResponse> GetChatResponse(string message)
        {
            return await AttemptContentGeneration(message, GenerateChatMessageResponse);
        }

        public async Task<GenerateTextResponse> GetTextContentFromPrompt(string prompt)
        {
            return await AttemptContentGeneration(prompt, GenerateTextFromPrompt);
        }
        
        private async Task<GenerateTextResponse> AttemptContentGeneration(string prompt, 
            Func<string, Task<GenerateTextResponse>> textGenerationFunc)
        {
            GenerateTextResponse? response;
            try
            {
                response = await textGenerationFunc(prompt);
            }
            catch (HttpRequestException httpRequestException) 
                when (httpRequestException.Message.Contains(InvalidAPIKeyExceptionContent))
            {
                await AttemptToFixAPIKey();
                response = await GetTextContentFromPrompt(prompt);
            }
            catch (Exception exception)
            {
                await Console.Error.WriteLineAsync($"Unexpected error occurred during content generation: {exception}");
                response = new GenerateTextResponse(text: null, error: exception.Message, isValid: false);
            }
            
            return response;
        }
        
        private async Task<GenerateTextResponse> GenerateChatMessageResponse(string message)
        {
            if (_chatSession == null)
                throw new InvalidOperationException(UninitializedModelExceptionMsg);
            
            var response = await _chatSession.SendMessage(message);
            return new GenerateTextResponse(response.Text);
        }

        private async Task<GenerateTextResponse> GenerateTextFromPrompt(string prompt)
        {
            if (_geminiModel == null)
                throw new InvalidOperationException(UninitializedModelExceptionMsg);
            
            var response = await _geminiModel.GenerateContent(prompt);
            return new GenerateTextResponse(response.Text);
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
