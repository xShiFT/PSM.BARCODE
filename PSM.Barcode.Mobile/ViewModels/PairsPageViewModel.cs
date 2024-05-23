using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PSM.Barcode.DB;
using PSM.Barcode.Services;
using System.Windows.Input;

namespace PSM.Barcode.ViewModels;

public class PairsPageViewModel: ObservableObject
{
	private readonly DbCtx _ctx;
	private readonly OptionsService _options;
	private readonly RestService _rest;

	public PairsPageViewModel(DbCtx ctx, OptionsService options, RestService rest)
    {
		_ctx = ctx;
		_options = options;
		_rest = rest;

		CmdClear   = new      RelayCommand(Clear);
		CmdUpdate  = new AsyncRelayCommand(Update);
		CmdMsgHide = new      RelayCommand(MessageHide);
	}


	private string _filter = "";
	public string Filter
	{
		get => _filter;
		set
		{
			var b = SetProperty(ref _filter, value.ToUpper());
			if (b)
				OnPropertyChanged(nameof(Items));
		}
	}

	public IEnumerable<PairViewModel> Items
	{
		get
		{
			Count = _ctx.Pairs
				.Where(p => Filter.Length == 0 || p.Barcode.Contains(Filter, StringComparison.CurrentCultureIgnoreCase) || p.Outcode.Contains(Filter, StringComparison.CurrentCultureIgnoreCase)).Count();

			return _ctx.Pairs
				.Where(p => Filter.Length == 0 || p.Barcode.Contains(Filter, StringComparison.CurrentCultureIgnoreCase) || p.Outcode.Contains(Filter, StringComparison.CurrentCultureIgnoreCase))
				.OrderByDescending(p => p.Barcode)
				.Take(100)
				.Select(p => new PairViewModel(p));
		}
	}

	private int _count = 0;
	public int Count
	{
		get => _count;
		set => SetProperty(ref _count, value);
	}


	private List<int> _sizes = [100, 250, 500, 1000, 2500, 5000];
	public List<int> Sizes => _sizes;

	public int PageSize
	{
		get => _options.PageSize;
		set
		{
			_options.PageSize = value;
			OnPropertyChanged();
		}
	}

	#region Commands

	public ICommand CmdClear { get; }
	private void Clear()
	{
		foreach (var pair in _ctx.Pairs)
			_ctx.Pairs.Remove(pair);
		_ctx.SaveChanges();
	}

	public ICommand CmdUpdate { get; }
	private async Task Update()
	{
		Message = "";
		
		var last = _ctx.Pairs.Max(p => p.Barcode) ?? "23000000";

		var result = await _rest.GetPairs(last, _options.PageSize);
		if (result.Error != null)
			Message = result.Error;
		else
		{
			var list = result.Value ?? [];
			foreach (var pair in list)
			{
				var p = _ctx.Pairs.FirstOrDefault(p => p.Barcode == pair.Barcode);
				if (p == null) _ctx.Pairs.Add(pair);
			}
			_ctx.SaveChanges();
		}
	}

	public ICommand CmdMsgHide { get; }
	private void MessageHide()
	{
		Message = string.Empty;
	}

	#endregion

	#region Message
	private string _message = string.Empty;
	public string Message
	{
		get => _message;
		private set
		{
			SetProperty(ref _message, value);
			OnPropertyChanged(nameof(MessageVisible));
		}
	}
	public bool MessageVisible => Message != string.Empty;
	#endregion
}
