using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PSM.Barcode.DB;
using PSM.Barcode.Models;
using PSM.Barcode.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PSM.Barcode.ViewModels;

public class BarcodesPageViewModel: ObservableObject
{
	private readonly OptionsService _options;
	private readonly DbCtx _ctx;
	private readonly BarcodesService _barcodes;
	private readonly RestService _rest;

	public BarcodesPageViewModel(OptionsService options, DbCtx ctx, BarcodesService barcodes, RestService rest)
	{
		_options = options;
		_ctx = ctx;
		_rest = rest;

		_barcodes = barcodes;
		_barcodes.Cleared    += BarcodesService_Cleared;
		_barcodes.Added      += BarcodesService_Added;
		_barcodes.Deleted    += BarcodesService_Deleted;
		_barcodes.Dublicated += BarcodesService_Dublicated;

		Barcodes   = new ObservableCollection<BarcodeViewModel>( _barcodes.Items );

		//CmdAdd     = new      RelayCommand(Add);
		CmdMsgHide = new      RelayCommand(MessageHide);
		CmdClear   = new AsyncRelayCommand(ItemsClear);
		CmdSend    = new AsyncRelayCommand(ItemsSend);
		CmdLast500 = new      RelayCommand(AddLast500);
	}

	#region BarcodesService Events

	private void BarcodesService_Deleted(object sender, ChangedBarcodeEventArgs e)
	{
		var item = Barcodes.FirstOrDefault(b => b.Item == e.Item);
		if (item == null) return;

		Barcodes.Remove(item);

		OnPropertyChanged(nameof(Barcodes));
		OnPropertyChanged(nameof(BarcodesReverse));
	}
	private void BarcodesService_Added(object sender, ChangedBarcodeEventArgs e)
	{
		Barcodes.Add(new(e.Item));

		OnPropertyChanged(nameof(Barcodes));
		OnPropertyChanged(nameof(BarcodesReverse));
	}
	private void BarcodesService_Cleared(object sender, EventArgs e)
	{
		Barcodes.Clear();

		OnPropertyChanged(nameof(Barcodes));
		OnPropertyChanged(nameof(BarcodesReverse));
	}
	private async void BarcodesService_Dublicated(object sender, ChangedBarcodeEventArgs e)
	{
		await Shell.Current.DisplayAlert("Штрихкоды", $"Повторное добавление: {e.Item.Barcode}", "Закрыть");
		//Message = $"{Message}\nПовтор: {e.Item.Barcode}".Trim();
		//await Shell.Current.DisplayAlert("Штрихкоды", $"Повторное добавление: {e.Item.Barcode}", "Закрыть");
	}

	#endregion

	public ObservableCollection<BarcodeViewModel> Barcodes { get; }
	public IEnumerable<BarcodeViewModel> BarcodesReverse => Barcodes.Reverse();

	#region Commands

	/*
	public ICommand CmdAdd { get; }
	private void Add()
	{
		//List<string> defaultBarcodes = ["23016371","23019710","23019759","23019965"];
		//foreach (var barcode in defaultBarcodes)
		//	_barcodes.Add(barcode);
		_barcodes.Add("23016371");
	}
	//*/

	public ICommand CmdClear { get; }
	private async Task ItemsClear()
	{
		bool answer = await Shell.Current.DisplayAlert("Штрихкоды", "Очистить список?", "Да", "Нет");
		if (answer) _barcodes.Clear();
	}

	public ICommand CmdSend { get; }
	private async Task<int> SMOP_Clear()
	{
		var result = await _rest.DeleteBarcodes(_options.SavedUserId);
		if (result != null && result.Error != "")
			Message = result.Error ?? "";
		return result?.Value ?? 0;
	}
	private async Task<int> SMOP_Send()
	{
		var result = await _rest.SendBarcodes(_options.SavedUserId, Barcodes.Select(b => b.Barcode));
		if (result != null && result.Error != "")
			Message = result.Error ?? "";
		return result?.Value ?? 0;
	}
	private async Task ItemsSend()
	{
		await SMOP_Clear();
		var inserted = await SMOP_Send();
		Message = (Message + $"\n\nУспешно добавлено: {inserted}.").Trim();
	}

	public ICommand CmdMsgHide { get; }
	private void MessageHide()
	{
		Message = string.Empty;
	}

	#endregion

	public ICommand CmdLast500 { get; }
	private void AddLast500()
	{
		_ctx.Items.RemoveRange(_ctx.Items);
		_ctx.SaveChanges();

		var list = _ctx.Pairs
			.OrderByDescending(p => p.Barcode)
			.Select(p => p.Barcode)
			.Take(500)
			.ToList()
			.OrderBy(b => b)
			.Select((b,i) => new BarcodeItem { ID = i + 1, Barcode = b })
			;
		_ctx.Items.AddRange(list);
		_ctx.SaveChanges();
	}

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
