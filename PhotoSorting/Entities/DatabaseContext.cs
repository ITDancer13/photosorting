using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PhotoSorting.Entities
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Image> Images { get; set; }
        public DbSet<Settings> Settings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=localStore.db");
        }

        public void EnsureDb()
        {
            Database.Migrate();
            foreach (var appliedMigration in Database.GetAppliedMigrations())
                Console.WriteLine(appliedMigration);

            // Ensure Settings Object
            if (!Settings.Any())
            {
                Settings.Add(new Settings());
                SaveChanges();
            }
        }
    }
}
