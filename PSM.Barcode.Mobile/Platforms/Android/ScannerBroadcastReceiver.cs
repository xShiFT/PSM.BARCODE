using Android.App;
using Android.Content;
using Android.Device;
using Android.Widget;

using PSM.Barcode.Services;

namespace PSM.Barcode.Scanner;

[BroadcastReceiver(Enabled = true, Exported = true)]
[IntentFilter([ScanManager.ActionDecode])]
public class ScannerBroadcastReceiver : BroadcastReceiver
{
	private readonly BarcodesService? _barcodes;
	private readonly OptionsService? _options;

	//

	public ScannerBroadcastReceiver()
	{}
	public ScannerBroadcastReceiver(BarcodesService? barcodes, OptionsService? options)
	{
		_barcodes = barcodes;
		_options = options;
	}

	//

	public override void OnReceive(Context? context, Intent? intent)
	{
		ArgumentNullException.ThrowIfNull(context);
		ArgumentNullException.ThrowIfNull(intent);

		// Do stuff here
		_ = intent.GetByteArrayExtra(ScanManager.DecodeDataTag);
		_ = intent.GetIntExtra(ScanManager.BarcodeLengthTag, 0);
		_ = intent.GetByteExtra(ScanManager.BarcodeTypeTag, 0x00);
		string result = (intent?.GetStringExtra(ScanManager.BarcodeStringTag) ?? "").Trim();

		if (result != null)
		{
			if (result.Length == 8)
				_barcodes?.Push(result);

			if (result.StartsWith("srv="))
			{
				result = result[4..];
				_options?.SetServer(result);
			}

			Toast.MakeText(context, result, ToastLength.Short)?.Show();
		}
	}
}
