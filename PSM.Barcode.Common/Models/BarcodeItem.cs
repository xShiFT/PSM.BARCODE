using System.ComponentModel.DataAnnotations;

namespace PSM.Barcode.Models;

public class BarcodeItem
{
	[Key]
	public int ID { get; set; }

	[MaxLength(8)]
	public required string Barcode { get; set; }
}
