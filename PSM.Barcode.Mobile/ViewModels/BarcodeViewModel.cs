using CommunityToolkit.Mvvm.ComponentModel;
using PSM.Barcode.DB;
using PSM.Barcode.Models;
namespace PSM.Barcode.ViewModels;

public class BarcodeViewModel(BarcodeItem item) : ObservableObject
{
	private readonly BarcodeItem _item = item;
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

}
