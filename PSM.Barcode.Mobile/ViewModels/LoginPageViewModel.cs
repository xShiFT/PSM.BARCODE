using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PSM.Barcode.DB;
using PSM.Barcode.Models;
using PSM.Barcode.Services;
using PSM.Barcode.Views;
using System.Windows.Input;

namespace PSM.Barcode.ViewModels;

public class LoginPageViewModel: ObservableObject
{
	private readonly OptionsService _options;
	private readonly DbCtx _ctx;
	private readonly RestService _rest;

	public LoginPageViewModel(OptionsService options, DbCtx ctx, RestService rest)
    {
		_ctx = ctx;
		_rest = rest;

		_options = options;
		_options.UserChanged += Options_UserChanged;

		CmdLogIn   = new AsyncRelayCommand(LogIn);
		CmdLogOut  = new      RelayCommand(LogOut);
	}

	#region Options Events

	private void Options_UserChanged(object sender, EventArgs e)
	{
		OnPropertyChanged(nameof(IsLogged));
		OnPropertyChanged(nameof(IsNotLogged));
		OnPropertyChanged(nameof(User));

		UserName = "";
		UserPass = "";
	}

	#endregion



	public bool IsLogged => _options.SavedUserId > 0;
	public bool IsNotLogged => _options.SavedUserId == 0;

	public User? User => _options.SavedUser;


	private string _userName = string.Empty;
	public string UserName
	{
		get => _userName;
		set => SetProperty(ref _userName, value);
	}

	private string _userPass = string.Empty;
	public string UserPass
	{
		get => _userPass;
		set => SetProperty(ref _userPass, value);
	}




	#region Commands

	public ICommand CmdLogIn { get; }

	private async Task<bool> UpdateUsers()
	{
		var result = await _rest.GetUsers();
		if (!string.IsNullOrWhiteSpace(result.Error))
		{
			await Shell.Current.DisplayAlert("Ошибка", result.Error, "Закрыть");
			return false;
		}
		else
		{
			foreach (var user in result.Value!)
			{
				user.Password = "";
				var u = _ctx.Users.FirstOrDefault(u => u.UserId == user.UserId);
				if (u != null)
				{
					u.Login      = user.Login;
					u.FirstName  = user.FirstName;
					u.LastName   = user.LastName;
					u.MiddleName = user.MiddleName;
				}
				else
				{
					_ctx.Users.Add(user);
				}
			}
			_ctx.SaveChanges();
		}
		return true;
	}
	private async Task LogIn()
	{
		var res = await UpdateUsers();
		if (!res)
		{
			return;
		}

		if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(UserPass))
		{
			await Shell.Current.DisplayAlert("Ошибка", "Имя и Пароль не должны быть пустыми!", "Закрыть");
			return;
		}

		var dto = new AuthDto { Login = UserName, Password = UserPass };

		var result = await _rest.LogIn(dto);

		if (!string.IsNullOrEmpty(result.Error))
		{
			await Shell.Current.DisplayAlert("Ошибка", result.Error, "Закрыть");
		}

		if (result.Value > 0)
		{
			_options.SetUserId(result.Value);
		}
	}

	public ICommand CmdLogOut { get; }
	private void LogOut()
	{
		_options.SetUserId(0);
	}

	public ICommand CmdToBarcodes { get; } = new AsyncRelayCommand(async () => await Shell.Current.GoToAsync(nameof(BarcodesPage)));

	#endregion
}
