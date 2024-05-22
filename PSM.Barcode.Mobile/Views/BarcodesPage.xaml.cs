using PSM.Barcode.ViewModels;

namespace PSM.Barcode.Views;

public partial class BarcodesPage : ContentPage
{
	public BarcodesPage(BarcodesPageViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}