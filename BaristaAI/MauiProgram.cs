using BaristaAI.Services;
using BaristaAI.Services.LLM;
using BaristaAI.ViewModel;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace BaristaAI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            // Register services
            builder.Services.AddSingleton<ILLMService, GeminiService>();
            builder.Services.AddSingleton<IAPIKeyService, APIKeyService>();
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddTransient<MainPage>();

            return builder.Build();
        }
    }
}
