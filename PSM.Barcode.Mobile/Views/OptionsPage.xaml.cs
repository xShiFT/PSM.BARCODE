using PSM.Barcode.ViewModels;

namespace PSM.Barcode.Views;

public partial class OptionsPage : ContentPage
{
	public OptionsPage(OptionsPageViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}