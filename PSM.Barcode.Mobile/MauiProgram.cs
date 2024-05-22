using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
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

			builder.Services.AddSingleton<MainPage>();
			builder.Services.AddSingleton<MainPageViewModel>();

			builder.Services.AddSingleton<LoginPage>();
			builder.Services.AddSingleton<LoginPageViewModel>();

			builder.Services.AddSingleton<OptionsPage>();
			builder.Services.AddSingleton<OptionsPageViewModel>();

			builder.Services.AddSingleton<BarcodesPage>();
			builder.Services.AddSingleton<BarcodesPageViewModel>();

			builder.Services.AddSingleton<PairsPage>();
			builder.Services.AddSingleton<PairsPageViewModel>();




#if DEBUG
			builder.Logging.AddDebug();
#endif

			return builder.Build();
		}
	}
}
