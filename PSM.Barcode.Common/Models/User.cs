using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PSM.Barcode.Models;

public class User
{
	[Key]
	public int UserId { get; set; }
	[MaxLength(20)]
	public required string Login { get; set; }
	[MaxLength(14)]
	public required string Password { get; set; }
	[MaxLength(15)]
	public required string FirstName { get; set; }
	[MaxLength(15)]
	public required string MiddleName { get; set; }
	[MaxLength(15)]
	public required string LastName { get; set; }

	[NotMapped]
	public string FullName => $"{FirstName} {MiddleName} {LastName}";
}
