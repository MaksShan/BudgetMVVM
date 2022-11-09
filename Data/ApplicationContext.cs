using BudgetMvvm.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetMVVM.Data
{
    internal sealed class ApplicationContext : DbContext
    {
        public DbSet<Operation>? Operations { get; set; }
        public DbSet<Category>? Categories { get; set; }

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=BudgetDB;Trusted_Connection=True");
        }
    }
}
