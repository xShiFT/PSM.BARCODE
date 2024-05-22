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

		CmdMsgHide = new RelayCommand(MessageHide);
		CmdClear   = new RelayCommand(ItemsClear);
		CmdSend    = new RelayCommand(ItemsSend);
	}

	#region _barcodes Events
	private void Barcodes_Deleted(object sender, ChangedBarcodeEventArgs e)
	{
		var item = Barcodes.FirstOrDefault(b => b.Item == e.Item);
		if (item == null) return;
		Barcodes.Remove(item);
	}
	private void Barcodes_Added(object sender, ChangedBarcodeEventArgs e)
	{
		Barcodes.Add(new(e.Item));
	}
	private void Barcodes_Cleared(object sender, EventArgs e)
	{
		Barcodes.Clear();
	}
	#endregion

	public List<BarcodeViewModel> Barcodes { get; }

	#region Commands

	public ICommand CmdClear { get; }
	public ICommand CmdSend { get; }
	public ICommand CmdMsgHide { get; }

	private void ItemsClear()
	{
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
