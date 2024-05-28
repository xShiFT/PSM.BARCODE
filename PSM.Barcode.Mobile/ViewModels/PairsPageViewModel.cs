using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PSM.Barcode.Services;
using System.Windows.Input;

namespace PSM.Barcode.ViewModels;

public class PairsPageViewModel: ObservableObject
{
	private readonly PairsService _pairs;
	private readonly OptionsService _options;
	private readonly RestService _rest;

	public PairsPageViewModel(PairsService pairs, OptionsService options, RestService rest)
    {
		_options = options;
		_rest = rest;

		_pairs = pairs;
		_pairs.Changed += (s,e) => OnPropertyChanged(nameof(Items));

		CmdClear   = new      RelayCommand(Clear);
		CmdUpdate  = new AsyncRelayCommand(Update);
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
			
			Count = _pairs.Items
				.Where(p => Filter.Length == 0 || p.Barcode.Contains(Filter, StringComparison.CurrentCultureIgnoreCase) || p.Outcode.Contains(Filter, StringComparison.CurrentCultureIgnoreCase))
				.Count();

			return _pairs.Items
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


	private readonly List<int> _sizes = [100, 250, 500, 1000, 2500, 5000];
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
		_pairs.Clear();
	}

	public ICommand CmdUpdate { get; }
	private async Task Update()
	{
		var last = _pairs.Items.Max(p => p.Barcode) ?? "23000000";

		var result = await _rest.GetPairs(last, _options.PageSize);
		if (result.Error != null)
			await Shell.Current.DisplayAlert("Ошибка", result.Error, "Закрыть");
		else
		{
			var list = result.Value ?? [];
			if (list.Count == 0)
			{
				await Shell.Current.DisplayAlert("Штрихкоды", "На текущий момент, все пары обновлены", "Закрыть");
			}
			/*
			foreach (var pair in list)
			{
				var p = _pairs.Items.FirstOrDefault(p => p.Barcode == pair.Barcode);
				if (p == null) _pairs.Add(pair.Barcode, pair.Outcode);
			}
			//*/
			_pairs.AddRange(list);
		}
	}

	#endregion
}
