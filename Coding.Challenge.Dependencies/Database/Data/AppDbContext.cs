using Coding.Challenge.Dependencies.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Coding.Challenge.Dependencies.Database.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) :
       base(options)
        { }

        public DbSet<Content> Contents { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
