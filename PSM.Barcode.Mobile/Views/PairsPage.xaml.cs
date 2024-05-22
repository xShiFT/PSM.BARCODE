using PSM.Barcode.ViewModels;

namespace PSM.Barcode.Views;

public partial class PairsPage : ContentPage
{
	public PairsPage(PairsPageViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}