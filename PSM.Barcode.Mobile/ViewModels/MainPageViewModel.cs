using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PSM.Barcode.Views;

using System.Windows.Input;

namespace PSM.Barcode.ViewModels;

public class MainPageViewModel: ObservableObject
{

	private readonly string _title = "PSM.Barcode";

	public string Title => _title;

	public MainPageViewModel()
	{
	}

	public ICommand CmdToBarcodes { get; } = new AsyncRelayCommand(async () => await Shell.Current.GoToAsync(nameof(BarcodesPage)));
	public ICommand CmdToLogins { get; } = new AsyncRelayCommand(async () => await Shell.Current.GoToAsync(nameof(LoginPage)));
	public ICommand CmdToOptions { get; } = new AsyncRelayCommand(async () => await Shell.Current.GoToAsync(nameof(OptionsPage)));
	public ICommand CmdToPairs { get; } = new AsyncRelayCommand(async () => await Shell.Current.GoToAsync(nameof(PairsPage)));
}
