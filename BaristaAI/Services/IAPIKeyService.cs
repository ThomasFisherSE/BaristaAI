namespace BaristaAI.Services
{
    internal interface IAPIKeyService
    {
        public Task<string> RequestAPIKey(string apiKeyName);

        public void RemoveCurrentAPIKey();
    }
}
