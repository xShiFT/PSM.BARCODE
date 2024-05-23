using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PSM.Barcode.DB;
using PSM.Barcode.Services;
using PSM.Barcode.Views;
using System.Windows.Input;

namespace PSM.Barcode.ViewModels;

public class MainPageViewModel: ObservableObject
{
	private readonly DbCtx _ctx;
	private readonly BarcodesService _barcodes;
	private readonly OptionsService _options;

	public MainPageViewModel(DbCtx ctx, BarcodesService barcodes, OptionsService options)
	{
		_ctx = ctx;

		_barcodes = barcodes;
		_barcodes.Changed += (s,e) => OnPropertyChanged(nameof(BarcodesCount));

		_options = options;
		_options.ServerChanged += (s, e) => OnPropertyChanged(nameof(Server));
		_options.UserChanged += (s, e) => OnPropertyChanged(nameof(User));
	}

	#region Commands

	public ICommand CmdToBarcodes { get; } = new AsyncRelayCommand(async () => await Shell.Current.GoToAsync(nameof(BarcodesPage)));
	public ICommand CmdToLogins { get; } = new AsyncRelayCommand(async () => await Shell.Current.GoToAsync(nameof(LoginPage)));
	public ICommand CmdToOptions { get; } = new AsyncRelayCommand(async () => await Shell.Current.GoToAsync(nameof(OptionsPage)));
	public ICommand CmdToPairs { get; } = new AsyncRelayCommand(async () => await Shell.Current.GoToAsync(nameof(PairsPage)));

	#endregion

	public int BarcodesCount => _barcodes.Count;
	public int PairsCount => _ctx.Pairs.Count();

	public string Server => _options.ServerName;
	public string User => _options.SavedUser?.FirstName ?? "не вошел";
}