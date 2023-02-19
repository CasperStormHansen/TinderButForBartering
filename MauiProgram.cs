using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Plugin.Firebase;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Shared;
#if IOS
    using Plugin.Firebase.iOS;
#else
    using Plugin.Firebase.Android;
#endif

namespace TinderButForBartering;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .RegisterFirebaseServices()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
			{
                fonts.AddFont("BaiJamjuree-Regular.ttf", "DefaultFontRegular");
                fonts.AddFont("BaiJamjuree-Medium.ttf", "DefaultFontBold");
                fonts.AddFont("FuzzyBubbles-Regular.ttf", "TitleFont");
            });

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}

    private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
    {
        builder.ConfigureLifecycleEvents(events => {
#if IOS
            events.AddiOS(iOS => iOS.FinishedLaunching((app, launchOptions) => {
                CrossFirebase.Initialize(app, launchOptions, CreateCrossFirebaseSettings());
                return false;
            }));
#else
            events.AddAndroid(android => android.OnCreate((activity, state) =>
                CrossFirebase.Initialize(activity, state, CreateCrossFirebaseSettings())));
#endif
        });

        builder.Services.AddSingleton(_ => CrossFirebaseAuth.Current);
        return builder;
    }

    private static CrossFirebaseSettings CreateCrossFirebaseSettings()
    {
        return new CrossFirebaseSettings(
            isAuthEnabled: true,
            facebookId: "524648662974660",
            facebookAppName: "BarterApp",
            googleRequestIdToken: "257597635890-evs65o3rrclu0oc5fqmgikv6gvcjlnj9.apps.googleusercontent.com"
        );
    }
}
