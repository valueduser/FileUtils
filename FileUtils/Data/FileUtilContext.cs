using FileUtil.Models;
using Microsoft.EntityFrameworkCore;

namespace FileUtils.Data
{
    public class FileUtilContext : DbContext
    {
        public DbSet<Hash> Hash { get; set; }
        public DbSet<File> File { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //TODO: don't hard code
            optionsBuilder.UseNpgsql(@"Host=localhost;Username=postgres;Password=postgres;Database=postgres");
        }
    }
}
