namespace BaristaAI.Services
{
    public interface ILLMService
    {
        /// <summary>
        /// Initializes a new model
        /// </summary>
        /// <param name="contextString">Optional string to provide initial context to the model</param>
        public void InitializeModel(string? contextString);

        /// <summary>
        /// Begins a new chat session
        /// </summary>
        /// <param name="initialModelMessage">Optional initial message the model should send</param>
        public void BeginNewChat(string? initialModelMessage = null);

        /// <summary>
        /// Get a response in the context of the current chat session (must have called <see cref="BeginNewChat(string?)"/> first)
        /// </summary>
        /// <param name="message">The chat message to send</param>
        /// <returns>A task returning the model's chat response</returns>
        public Task<string?> GetChatResponse(string message);

        /// <summary>
        /// Get a text response for a given prompt
        /// </summary>
        /// <param name="prompt">The prompt string</param>
        /// <returns>A task returning the model's text response</returns>
        public Task<string?> GetTextResponse(string prompt);
    }
}
