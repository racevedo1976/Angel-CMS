using Microsoft.EntityFrameworkCore;
using Angelo.Connect.NavMenu.Models;

namespace Angelo.Connect.NavMenu.Data
{
    public class NavMenuDbContext : DbContext
    {
        public DbSet<NavMenuWidget> NavMenuWidgets { get; set; }

        public NavMenuDbContext(DbContextOptions<NavMenuDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureWidgetModels(modelBuilder);
        }

        private static void ConfigureWidgetModels(ModelBuilder builder)
        {
            builder.Entity<NavMenuWidget>(entity =>
            {
                entity.ToTable("NavMenuWidget", "plugin").HasKey(e => e.Id);
            });
        }
    }
}
