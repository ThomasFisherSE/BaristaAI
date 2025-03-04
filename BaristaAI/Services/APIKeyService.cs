using BaristaAI.View;
using CommunityToolkit.Maui.Views;
using Microsoft.Extensions.Configuration;

namespace BaristaAI.Services
{
    internal class APIKeyService : IAPIKeyService
    {
        private const string APIKeySecureStorageKey = "BARISTAAI_APIKEY";

        public async Task<string> RequestAPIKey(string apiKeyName)
        {
            // Try to obtain API key from user secrets configuration
            var builder = new ConfigurationBuilder().AddUserSecrets<APIKeyService>();
            var configuration = builder.Build();
            
            var apiKey = configuration[apiKeyName];

            // If not found, try to obtain API key from secure storage
            if (string.IsNullOrEmpty(apiKey))
            {
                apiKey = await SecureStorage.GetAsync(APIKeySecureStorageKey);
            }

            // If still not found, ask user to enter an API key
            while (string.IsNullOrEmpty(apiKey))
                apiKey = await AskUserForAPIKey();

            // Store the API key in secure storage so that it can be used next time
            await SecureStorage.SetAsync(APIKeySecureStorageKey, apiKey);

            return apiKey;
        }

        private static async Task<string?> AskUserForAPIKey()
        {
            var mainPage = Application.Current?.MainPage;
            if (mainPage == null)
                return null;

            var apiKeyPopup = new APIKeyPopup();
            return await mainPage.ShowPopupAsync(apiKeyPopup) as string;
        }

        public void RemoveCurrentAPIKey()
        {
            SecureStorage.Remove(APIKeySecureStorageKey);
        }
    }
}
