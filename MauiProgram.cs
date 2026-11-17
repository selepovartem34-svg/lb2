using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

namespace MyXmlMAUIApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if WINDOWS
        // Підтвердження виходу і при натисканні на хрестик вікна
        builder.ConfigureLifecycleEvents(events =>
        {
            events.AddWindows(windows => windows.OnWindowCreated(window =>
            {
                window.AppWindow.Closing += async (sender, args) =>
                {
                    if (App.IsExiting)
                        return;

                    args.Cancel = true;
                    var page = Application.Current?.Windows.FirstOrDefault()?.Page;
                    if (page != null)
                        await App.ConfirmExitAsync(page);
                };
            }));
        });
#endif

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
