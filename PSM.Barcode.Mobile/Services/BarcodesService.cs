using PSM.Barcode.DB;
using PSM.Barcode.Models;
using PSM.Barcode.ViewModels;

namespace PSM.Barcode.Services;

public class BarcodesService(DbCtx ctx)
{
	private readonly DbCtx _ctx = ctx;

	public IEnumerable<BarcodeViewModel> Items => _ctx.Items.Select(item => new BarcodeViewModel(item));

	public int Count => _ctx.Items.Count();

	public void Add(string barcode)
	{
		var last  = _ctx.Items.Any() ? _ctx.Items.Max(itm => itm.ID) : 0;

		var item = _ctx.Items.FirstOrDefault(itm => itm.Barcode == barcode);
		if (item != null)
		{
			Dublicated?.Invoke(this, new (item));
			return;
		}

		item = new BarcodeItem { ID = last + 1, Barcode = barcode };
		_ctx.Items.Add(item);
		_ctx.SaveChanges();

		Added?.Invoke(this, new (item));
		Changed?.Invoke(this, new ());
	}
	public void Delete(string barcode)
	{
		var item = _ctx.Items.FirstOrDefault(itm => itm.Barcode == barcode);
		if (item == null) return;

		_ctx.Items.Remove(item);
		_ctx.SaveChanges();

		Deleted?.Invoke(this, new (item));
		Changed?.Invoke(this, new ());
	}
	public void Clear()
	{
		foreach (var item in _ctx.Items)
			_ctx.Items.Remove(item);
		_ctx.SaveChanges();

		Cleared?.Invoke(this, new ());
		Changed?.Invoke(this, new ());
	}

	public delegate void DublicatedEventHandler(object sender, ChangedBarcodeEventArgs e);
	public delegate void AddedEventHandler(object sender, ChangedBarcodeEventArgs e);
	public delegate void DeletedEventHandler(object sender, ChangedBarcodeEventArgs e);
	public delegate void ClearedEventHandler(object sender, EventArgs e);
	public delegate void ChangedEventHandler(object sender, EventArgs e);

	public event DublicatedEventHandler? Dublicated;
	public event AddedEventHandler? Added;
	public event DeletedEventHandler? Deleted;
	public event ClearedEventHandler? Cleared;
	public event ChangedEventHandler? Changed;
}

public class ChangedBarcodeEventArgs(BarcodeItem item)
{
	public BarcodeItem Item { get; } = item;
}