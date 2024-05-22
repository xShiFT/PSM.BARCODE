using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PSM.Barcode.DB;
using PSM.Barcode.Models;
using PSM.Barcode.Services;
using System.Windows.Input;
namespace PSM.Barcode.ViewModels;

public class BarcodeViewModel : ObservableObject
{
	private readonly BarcodeItem _item;
	private readonly BarcodesService? _barcodes;

	public BarcodeViewModel(BarcodeItem item)
    {
		_item = item;

		_barcodes = ServiceProvider.GetService<BarcodesService>();

		CmdRemove = new RelayCommand(Remove);
	}

    public BarcodeItem Item => _item;

	public int ID => _item.ID;
	public string Barcode => _item.Barcode;

	private string _outcode = string.Empty;
	public string Outcode
	{
		get
		{
			if (_outcode == string.Empty)
			{
				using (var ctx = new DbCtx())
				{
					_outcode = ctx.Pairs.FirstOrDefault(p => p.Barcode == Barcode)?.Outcode ?? "";
				}
			}
			return _outcode;
		}
	}

	public ICommand CmdRemove { get; }
	private void Remove()
	{
		_barcodes?.Delete(Barcode);
	}
}
