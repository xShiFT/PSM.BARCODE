using System.ComponentModel.DataAnnotations;

namespace PSM.Barcode.Models;

public class AuthDto
{
	[MaxLength(20)]
	public required string Login { get; set; }
	[MaxLength(20)]
	public required string Password { get; set; }
}
