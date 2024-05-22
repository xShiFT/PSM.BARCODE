using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PSM.Barcode.Services;

using System.Windows.Input;

namespace PSM.Barcode.ViewModels;

public class BarcodesPageViewModel: ObservableObject
{
	private readonly BarcodesService _barcodes;

	public BarcodesPageViewModel(BarcodesService barcodes)
	{
		_barcodes = barcodes;
		_barcodes.Cleared += Barcodes_Cleared;
		_barcodes.Added   += Barcodes_Added;
		_barcodes.Deleted += Barcodes_Deleted;

		Barcodes = _barcodes.Items.ToList();

		CmdAdd = new RelayCommand(Add);
		CmdMsgHide = new RelayCommand(MessageHide);
		CmdClear   = new AsyncRelayCommand(ItemsClear);
		CmdSend    = new RelayCommand(ItemsSend);
	}

	#region _barcodes Events
	private void Barcodes_Deleted(object sender, ChangedBarcodeEventArgs e)
	{
		var item = Barcodes.FirstOrDefault(b => b.Item == e.Item);
		if (item == null) return;
		Barcodes.Remove(item);
		OnPropertyChanged(nameof(Barcodes));
	}
	private void Barcodes_Added(object sender, ChangedBarcodeEventArgs e)
	{
		Barcodes.Add(new(e.Item));
		OnPropertyChanged(nameof(Barcodes));
	}
	private void Barcodes_Cleared(object sender, EventArgs e)
	{
		Barcodes.Clear();
		OnPropertyChanged(nameof(Barcodes));
	}
	#endregion

	public List<BarcodeViewModel> Barcodes { get; }

	#region Commands
	public ICommand CmdAdd { get; }
	public ICommand CmdClear { get; }
	public ICommand CmdSend { get; }
	public ICommand CmdMsgHide { get; }

	private void Add()
	{
		List<string> defaultBarcodes = new()
		{
			"23016371",
			"23019710",
			"23019759",
			"23019965",
		};
		foreach (var barcode in defaultBarcodes)
			_barcodes.Add(barcode);
	}
	private async Task ItemsClear()
	{
		bool answer = await Shell.Current.DisplayAlert("Штрихкоды", "Очистить список?", "Да", "Нет");
		if (answer) _barcodes.Clear();
	}
	private void ItemsSend()
	{
	}
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
