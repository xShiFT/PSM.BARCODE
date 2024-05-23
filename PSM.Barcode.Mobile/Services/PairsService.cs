using PSM.Barcode.DB;
using PSM.Barcode.Models;
using PSM.Barcode.ViewModels;

namespace PSM.Barcode.Services;

public class PairsService(DbCtx ctx)
{
	private readonly DbCtx _ctx = ctx;

	public IEnumerable<BarcodePair> Items => _ctx.Pairs.AsEnumerable();
	public int Count => Items.Count();

	public void Add(string barcode, string outcode)
	{
		var item = _ctx.Pairs.FirstOrDefault(itm => itm.Barcode == barcode);
		if (item != null)
		{
			Dublicated?.Invoke(this, new (item));
			return;
		}

		item = new BarcodePair { Barcode = barcode, Outcode = outcode };
		_ctx.Pairs.Add(item);
		_ctx.SaveChanges();

		Added?.Invoke(this, new (item));
		Changed?.Invoke(this, new ());
	}
	public void Delete(string barcode)
	{
		var item = _ctx.Pairs.FirstOrDefault(itm => itm.Barcode == barcode);
		if (item == null) return;

		_ctx.Pairs.Remove(item);
		_ctx.SaveChanges();

		Deleted?.Invoke(this, new (item));
		Changed?.Invoke(this, new ());
	}
	public void Clear()
	{
		foreach (var item in _ctx.Pairs)
			_ctx.Pairs.Remove(item);
		_ctx.SaveChanges();

		Cleared?.Invoke(this, new ());
		Changed?.Invoke(this, new ());
	}

	public delegate void DublicatedEventHandler(object sender, ChangedPairEventArgs e);
	public delegate void AddedEventHandler(object sender, ChangedPairEventArgs e);
	public delegate void DeletedEventHandler(object sender, ChangedPairEventArgs e);
	public delegate void ClearedEventHandler(object sender, EventArgs e);
	public delegate void ChangedEventHandler(object sender, EventArgs e);

	public event DublicatedEventHandler? Dublicated;
	public event AddedEventHandler? Added;
	public event DeletedEventHandler? Deleted;
	public event ClearedEventHandler? Cleared;
	public event ChangedEventHandler? Changed;
}

public class ChangedPairEventArgs(BarcodePair item)
{
	public BarcodePair Item { get; } = item;
}