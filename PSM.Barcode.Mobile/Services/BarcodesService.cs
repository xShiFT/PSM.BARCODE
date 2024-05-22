using PSM.Barcode.DB;
using PSM.Barcode.Models;
using PSM.Barcode.ViewModels;

namespace PSM.Barcode.Services;

public class BarcodesService(DbCtx ctx)
{
	private readonly DbCtx _ctx = ctx;

	public IEnumerable<BarcodeViewModel> GetItems() => _ctx.Items.Select(item => new BarcodeViewModel(item));

	public void Push(string barcode)
	{
		var item = _ctx.Items.FirstOrDefault(itm => itm.Barcode == barcode);
		if (item != null) return;

		item = new BarcodeItem { Barcode = barcode };
		_ctx.Items.Add(item);
		_ctx.SaveChanges();

		Added?.Invoke(this, new ChangedBarcodeEventArgs(item));
	}
	public void Delete(string barcode)
	{
		var item = _ctx.Items.FirstOrDefault(itm => itm.Barcode == barcode);
		if (item == null) return;
		_ctx.Items.Remove(item);
		_ctx.SaveChanges();
		Deleted?.Invoke(this, new ChangedBarcodeEventArgs(item));
	}
	public void Clear()
	{
		foreach (var item in _ctx.Items)
			_ctx.Items.Remove(item);
		_ctx.SaveChanges();

		Cleared?.Invoke(this, new EventArgs());
	}

	public delegate void AddedEventHandler(object sender, ChangedBarcodeEventArgs e);
	public delegate void DeletedEventHandler(object sender, ChangedBarcodeEventArgs e);
	public delegate void ClearedEventHandler(object sender, EventArgs e);

	public event AddedEventHandler? Added;
	public event DeletedEventHandler? Deleted;
	public event ClearedEventHandler? Cleared;
}

public class ChangedBarcodeEventArgs(BarcodeItem item)
{
	public BarcodeItem Item { get; } = item;
}