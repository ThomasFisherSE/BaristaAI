namespace BaristaAI.Services.LLM
{
    public interface ILLMService
    {
        /// <summary>
        /// Initializes a new model
        /// </summary>
        /// <param name="contextString">Optional string to provide initial context to the model</param>
        public Task InitializeModel(string? contextString = null);

        /// <summary>
        /// Starts a new chat session
        /// </summary>
        /// <param name="initialModelMessage">Optional initial message the model should send</param>
        public void StartNewChatSession(string? initialModelMessage = null);

        /// <summary>
        /// Generate text in response to a chat message within the context of the current chat session
        /// </summary>
        /// <param name="message">The chat message to generate a response to</param>
        /// <returns>A task returning the model's chat response</returns>
        public Task<string?> GetChatResponse(string message);

        /// <summary>
        /// Generate text content from a given prompt
        /// </summary>
        /// <param name="prompt">The prompt string</param>
        /// <returns>A task returning the model's text response</returns>
        public Task<string?> GetTextContentFromPrompt(string prompt);
    }
}
