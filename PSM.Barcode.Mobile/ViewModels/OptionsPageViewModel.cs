using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PSM.Barcode.Services;
using System.Windows.Input;

namespace PSM.Barcode.ViewModels;

public class OptionsPageViewModel: ObservableObject
{
	private readonly OptionsService _options;
	private readonly RestService _rest;

	public OptionsPageViewModel(OptionsService options, RestService rest)
    {
		_rest = rest;

		_options = options;
		_options.ServerChanged += Options_ServerChanged;

		//CmdDefaultValue   = new      RelayCommand(() => _options.SetServer("http://isvt-44:60100|http://192.168.6.111:60100"));
		CmdConnectionTest = new AsyncRelayCommand(ConnectionTest);
	}

	#region Options Events

	private void Options_ServerChanged(object sender, EventArgs e)
	{
		OnPropertyChanged(nameof(ServerHost));
		OnPropertyChanged(nameof(ServerIP));
	}

	#endregion

	#region Options

	public string ServerHost => _options.ServerHost;
	public string ServerIP => _options.ServerIP;

	public bool UseHost
	{
		get => _options.UseHost;
		set => _options.UseHost = value;
	}

	#endregion

	#region Commands

	//public ICommand CmdDefaultValue { get; }

	public ICommand CmdConnectionTest { get; }
	private async Task ConnectionTest()
	{
		var result = await _rest.Test();
		if (result.Error != null)
			await Shell.Current.DisplayAlert("Ошибка", result.Error, "Закрыть");
		else
			await Shell.Current.DisplayAlert("Штрихкоды", $"HTTP CODE: {result.StatusCode}\nTEST OK", "Закрыть");
	}

	#endregion
}