using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PSM.Barcode.Models;

[Table("Basket_dev")]
public class BasketItem
{
	public int IdU { get; set; }
	public int IdO { get; set; }

	[Key, MaxLength(8)]
	public required string BarCode { get; set; }
}
