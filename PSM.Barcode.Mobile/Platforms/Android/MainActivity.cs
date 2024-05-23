using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Device;
using PSM.Barcode.Scanner;
using PSM.Barcode.Services;

namespace PSM.Barcode;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
	private readonly ScannerBroadcastReceiver receiver;
	private readonly ScanManager manager;

	//BarcodesService service
	public MainActivity()
	{
		var barcodes = ServiceProvider.GetService<BarcodesService>();
		var options = ServiceProvider.GetService<OptionsService>();

		manager  = new ScanManager();
		receiver = new ScannerBroadcastReceiver(barcodes, options);
	}

	protected override void OnResume()
	{
		base.OnResume();

		RegisterReceiver(receiver, new IntentFilter(ScanManager.ActionDecode));
		manager.OpenScanner();
	}
	protected override void OnPause()
	{
		base.OnPause();

		UnregisterReceiver(receiver);
		manager.CloseScanner();
	}
}
