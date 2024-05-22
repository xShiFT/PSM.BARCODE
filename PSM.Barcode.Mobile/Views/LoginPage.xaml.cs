using PSM.Barcode.ViewModels;

namespace PSM.Barcode.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginPageViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}