using Microsoft.EntityFrameworkCore;

using PSM.Barcode.Models;

namespace PSM.Barcode.DB;

public class DbCtx : DbContext
{
    public DbCtx()
    {
        SQLitePCL.Batteries_V2.Init();

        Database.EnsureCreated();

		InitDefaultValues();
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PSM.Barcode.db3");

		optionsBuilder.UseSqlite($"Filename={path}");
	}

	public DbSet<User> Users { get; set; }
	public DbSet<BarcodeItem> Items { get; set; }
	public DbSet<BarcodePair> Pairs { get; set; }



	private void InitDefaultValues()
	{
		Dictionary<string, string> DefaultPairs = new()
		{
			{"23016371","333.3.55.100.220"},
			{"23019710","210.4.12.01.03"},
			{"23019759","310.4.80.05.06"},
			{"23019965","310.4.112.03.06."},
		};

		if (!Pairs.Any())
		{
			var i = 0;
			foreach (var pair in DefaultPairs)
			{
				Pairs.Add(new BarcodePair { Barcode = pair.Key, Outcode = pair.Value });
				Items.Add(new BarcodeItem { ID = ++i, Barcode = pair.Key });
			}
			SaveChanges();
		}
	}
}
