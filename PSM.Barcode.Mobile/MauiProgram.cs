using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using PSM.Barcode.DB;
using PSM.Barcode.Services;
using PSM.Barcode.ViewModels;
using PSM.Barcode.Views;

namespace PSM.Barcode
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

			builder.Services.AddDbContext<DbCtx>();
			builder.Services.AddSingleton<BarcodesService>();
			builder.Services.AddSingleton<PairsService>();
			builder.Services.AddSingleton(Preferences.Default);
			builder.Services.AddScoped<OptionsService>();
			builder.Services.AddScoped<RestService>();

			builder.Services.AddTransient<MainPage>();
			builder.Services.AddTransient<LoginPage>();
			builder.Services.AddTransient<OptionsPage>();
			builder.Services.AddTransient<BarcodesPage>();
			builder.Services.AddTransient<PairsPage>();

			builder.Services.AddTransient<MainPageViewModel>();
			builder.Services.AddTransient<LoginPageViewModel>();
			builder.Services.AddTransient<OptionsPageViewModel>();
			builder.Services.AddTransient<BarcodesPageViewModel>();
			builder.Services.AddTransient<PairsPageViewModel>();

#if DEBUG
			builder.Logging.AddDebug();
#endif

			return builder.Build();
		}
	}
}
