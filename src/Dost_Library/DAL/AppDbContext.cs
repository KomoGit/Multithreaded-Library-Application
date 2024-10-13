using Dost_Library.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Dost_Library.DAL
{
    public sealed class AppDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        //Please forgive me for this abominable path code I have written,
        //It is the only way I can ensure a consistent database location,
        //Otherwise it (database.db) will end up being generated in bin folder.
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"DataSource={Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent}\\database.db");
        }
    }
}
