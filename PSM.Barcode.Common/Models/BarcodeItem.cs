using System.ComponentModel.DataAnnotations;

namespace PSM.Barcode.Models;

public class BarcodeItem
{
	[Key]
	public int ID { get; set; }

	[MaxLength(8)]
	public required string Barcode { get; set; }

	/*
	[MaxLength(50)]
	public required string Outcode { get; set; }

	[NotMapped]
	public required string Outcode1
	{
		get
		{

		}
	}
	//*/
}
