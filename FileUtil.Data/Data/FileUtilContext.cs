using FileUtil.Models;
using Microsoft.EntityFrameworkCore;
using File = FileUtil.Models.File;

namespace FileUtil.Data.Data
{
	public class FileUtilContext : DbContext
	{
		public DbSet<Hash> Hash { get; set; } = null!;
		public DbSet<File> File { get; set; } = null!;


		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			//TODO: don't hard code
			optionsBuilder.UseNpgsql(@"Host=localhost;port=5432;Username=postgres;Password=postgres;Database=postgres");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<File>()
				.HasOne<Hash>(s => s.Hash)
				.WithMany(g => g.Files)
				.HasForeignKey(s => s.HashId);
		}
	}
}
