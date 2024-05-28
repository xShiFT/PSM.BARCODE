using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PSM.Barcode.Services;
using PSM.Barcode.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PSM.Barcode.ViewModels;

public class BarcodesPageViewModel: ObservableObject
{
	private readonly OptionsService _options;
	private readonly BarcodesService _barcodes;
	private readonly RestService _rest;

	public BarcodesPageViewModel(OptionsService options, BarcodesService barcodes, RestService rest)
	{
		_options = options;
		_rest = rest;

		_barcodes = barcodes;
		_barcodes.Cleared    += BarcodesService_Cleared;
		_barcodes.Added      += BarcodesService_Added;
		_barcodes.Deleted    += BarcodesService_Deleted;
		_barcodes.Dublicated += BarcodesService_Dublicated;

		Barcodes   = new ObservableCollection<BarcodeViewModel>( _barcodes.Items );

		//CmdAdd     = new      RelayCommand(Add);
		CmdClear   = new AsyncRelayCommand(ItemsClear);
		CmdSend    = new AsyncRelayCommand(ItemsSend);
	}

	#region BarcodesService Events

	private void BarcodesService_Deleted(object sender, ChangedBarcodeEventArgs e)
	{
		var item = Barcodes.FirstOrDefault(b => b.Item == e.Item);
		if (item == null) return;

		Barcodes.Remove(item);

		OnPropertyChanged(nameof(Barcodes));
	}
	private void BarcodesService_Added(object sender, ChangedBarcodeEventArgs e)
	{
		Barcodes.Add(new(e.Item));

		OnPropertyChanged(nameof(Barcodes));
	}
	private void BarcodesService_Cleared(object sender, EventArgs e)
	{
		Barcodes.Clear();

		OnPropertyChanged(nameof(Barcodes));
	}
	private async void BarcodesService_Dublicated(object sender, ChangedBarcodeEventArgs e)
	{
		await Shell.Current.DisplayAlert("Штрихкоды", $"Повторное добавление: {e.Item.Barcode}", "Закрыть");
	}

	#endregion

	public ObservableCollection<BarcodeViewModel> Barcodes { get; }

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
		if (answer)
		{
			_barcodes.Clear();
		}
	}

	public ICommand CmdSend { get; }
	private async Task<(bool,int)> SMOP_Clear()
	{
		var result = await _rest.DeleteBarcodes(_options.SavedUserId);
		if (result != null && !string.IsNullOrEmpty(result.Error))
		{
			await Shell.Current.DisplayAlert("Ошибка", result.Error, "Закрыть");
			return (false, 0);
		}
		return (true,result?.Value ?? 0);
	}
	private async Task<(bool,int)> SMOP_Send()
	{
		var result = await _rest.SendBarcodes(_options.SavedUserId, Barcodes.Select(b => b.Barcode));
		if (result != null && !string.IsNullOrEmpty(result.Error))
		{
			await Shell.Current.DisplayAlert("Ошибка", result.Error, "Закрыть");
			return (false, 0);
		}
		return (true, result?.Value ?? 0);
	}
	private async Task ItemsSend()
	{
		if (_barcodes.Count == 0)
		{
			await Shell.Current.DisplayAlert("Штрихкоды", "Не чего отправлять", "Закрыть");
			return;
		}
		if (_options.SavedUserId == 0)
		{
			bool resl = await Shell.Current.DisplayAlert("Штрихкоды", "Перед отправкой, вам следует авторизоваться", "Перейти", "Закрыть");
			if (resl)
			{
				await Shell.Current.GoToAsync(nameof(LoginPage));
			}
			return;
		}
		var (resc,_) = await SMOP_Clear();
		if (!resc)
		{
			return;
		}
		var (resi,inserted) = await SMOP_Send();
		if (!resi)
		{
			return;
		}
		if (inserted > 0)
		{
			await Shell.Current.DisplayAlert("Штрихкоды", $"Успешно отправлено: {inserted}.", "Закрыть");
		}
	}

	#endregion
}
