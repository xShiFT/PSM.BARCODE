using CommunityToolkit.Mvvm.ComponentModel;
using PSM.Barcode.Models;

namespace PSM.Barcode.ViewModels;

public class PairViewModel (BarcodePair pair): ObservableObject
{
	private readonly BarcodePair _pair = pair;

	public string Barcode => _pair.Barcode;
	public string Outcode => _pair.Outcode;
}
