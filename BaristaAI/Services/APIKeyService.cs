using BaristaAI.View;
using CommunityToolkit.Maui.Views;
using Microsoft.Extensions.Configuration;

namespace BaristaAI.Services
{
    internal class APIKeyService : IAPIKeyService
    {
        public async Task<string> RequestAPIKey(string apiKeyName)
        {
            var builder = new ConfigurationBuilder().AddUserSecrets<APIKeyService>();
            var configuration = builder.Build();
            
            var apiKey = configuration[apiKeyName];

            while (string.IsNullOrEmpty(apiKey))
                apiKey = await AskUserForAPIKey();

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
    }
}
