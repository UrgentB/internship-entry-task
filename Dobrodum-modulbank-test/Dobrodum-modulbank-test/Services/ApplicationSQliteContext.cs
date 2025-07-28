using Dobrodum_modulbank_test.Models;
using Microsoft.EntityFrameworkCore;

namespace Dobrodum_modulbank_test.Services
{
    public class ApplicationSQliteContext : DbContext
    {
        private string DataBasePath = "Data Source=CrestNullGame.sqlite3";

        public DbSet<CrestNullGame> Games { get; set; } = null!;
        public ApplicationSQliteContext() => Database.EnsureCreated();

        public ApplicationSQliteContext(string DataBasePath)
        {
            this.DataBasePath = DataBasePath;
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite(DataBasePath);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CrestNullGame>();
        }
    }
}
