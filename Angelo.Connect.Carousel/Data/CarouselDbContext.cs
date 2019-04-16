using Microsoft.EntityFrameworkCore;
using Angelo.Connect.Carousel.Models;

namespace Angelo.Connect.Carousel.Data
{
    public class CarouselDbContext : DbContext
    {
        public DbSet<CarouselWidget> CarouselWidgets { get; set; }

        public DbSet<CarouselSlide> CarouselSlides { get; set; }
        //public DbSet<DocumentListFolder> DocumentListFolders { get; set; }

        public CarouselDbContext(DbContextOptions<CarouselDbContext> options) : base(options)
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
            builder.Entity<CarouselWidget>(entity =>
            {
                entity.ToTable("CarouselWidget", "plugin").HasKey(e => e.Id);
                entity
                    .HasMany(x => x.Slides)
                    .WithOne(x => x.Widget)
                    .HasForeignKey(x => x.WidgetId);
               
            });

            builder.Entity<CarouselSlide>(entity =>
            {
                entity.ToTable("CarouselSlide", "plugin").HasKey(e => e.Id);
               
            });

            
        }
    }
}
