using System.ComponentModel.DataAnnotations;

namespace PSM.Barcode.Models;

public class BarcodePair
{
	[Key, MaxLength(8)]
	public required string Barcode { get; set; }
	[MaxLength(50)]
	public required string Outcode { get; set; }
}