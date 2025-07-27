using Microsoft.EntityFrameworkCore;

namespace Dobrodum_modulbank_test
{
    public class ApplicationSQliteContext : DbContext
    {
        private const string DataBasePath = "Data Source=CrestNullGame.sqlite3";

        public DbSet<CrestNullGame> Games { get; set; } = null!;
        public ApplicationSQliteContext() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite(DataBasePath);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CrestNullGame>();
        }
    }
}
