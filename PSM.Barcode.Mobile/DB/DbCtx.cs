using Microsoft.EntityFrameworkCore;

using PSM.Barcode.Models;

namespace PSM.Barcode.DB;

public class DbCtx : DbContext
{
    public DbCtx()
    {
        SQLitePCL.Batteries_V2.Init();

        Database.EnsureCreated();
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PSM.Barcode.db3");

		optionsBuilder.UseSqlite($"Filename={path}");
	}

	public DbSet<User> Users { get; set; }
	public DbSet<BarcodeItem> Items { get; set; }
	public DbSet<BarcodePair> Pairs { get; set; }
}
