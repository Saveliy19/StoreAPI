using DAL.DataBase.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace DAL.DataBase
{
    public class StoreDbContext: DbContext
    {
        private readonly string _connectionString;
        public DbSet<Models.Store> Stores { get; set; }
        public DbSet<Models.Product> Products { get; set; }
        public DbSet<Models.StoreProduct> StoreProducts { get; set; }

        public StoreDbContext(string connectionString)
        {
            _connectionString = connectionString;
            Database.EnsureCreatedAsync();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={_connectionString}"); // заменить на параметр из аппсеттингс
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StoreProduct>()
                .HasKey(sp => new { sp.StoreId, sp.ProductName });

            modelBuilder.Entity<StoreProduct>()
                .HasOne(sp => sp.Store)
                .WithMany(s => s.StoreProducts)
                .HasForeignKey(sp => sp.StoreId);

            modelBuilder.Entity<StoreProduct>()
                .HasOne(sp => sp.Product)
                .WithMany(p => p.StoreProducts)
                .HasForeignKey(sp => sp.ProductName);
        }
    }
}
