using Microsoft.EntityFrameworkCore;

using Angelo.Connect.News.Models;

namespace Angelo.Connect.News.Data
{
    public class NewsDbContext : DbContext
    {
        public DbSet<NewsCategory> NewsCategories { get; set; }
        public DbSet<NewsPost> NewsPosts { get; set; }
        public DbSet<NewsPostCategory> NewsPostCategories { get; set; }
        public DbSet<NewsPostTag> NewsPostTags { get; set; }
        public DbSet<NewsWidget> NewsWidgets { get; set; }
        public DbSet<NewsWidgetCategory> NewsWidgetCategories { get; set; }
        public DbSet<NewsWidgetTag> NewsWidgetTags { get; set; }

        public NewsDbContext(DbContextOptions<NewsDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureNewsModels(modelBuilder);
            ConfigureWidgetModels(modelBuilder);
        }

        private static void ConfigureNewsModels(ModelBuilder builder)
        {
            builder.Entity<NewsCategory>(entity => {
                entity.ToTable("NewsCategory", "plugin").HasKey(e => e.Id);

                entity.HasMany(x => x.NewsPostMap).WithOne(x => x.Category);
            });

            builder.Entity<NewsPost>(entity => {
                entity.ToTable("NewsPost", "plugin").HasKey(e => e.Id);

                entity
                    .HasMany(x => x.Categories)
                    .WithOne(x => x.Post)
                    .HasForeignKey(x => x.PostId);

                entity
                    .HasMany(x => x.Tags)
                    .WithOne(x => x.Post)
                    .HasForeignKey(x => x.PostId);
            });

            builder.Entity<NewsPostCategory>(entity => {
                entity.ToTable("NewsPostCategory", "plugin").HasKey(e => e.Id);
            });

            builder.Entity<NewsPostTag>(entity => {
                entity.ToTable("NewsPostTag", "plugin").HasKey(e => e.Id);
            });
        }

        private static void ConfigureWidgetModels(ModelBuilder builder)
        {         
            builder.Entity<NewsWidget>(entity => {
                entity.ToTable("NewsWidget", "plugin").HasKey(e => e.Id);
             
                entity
                    .HasMany(x => x.Categories)
                    .WithOne(x => x.Widget)
                    .HasForeignKey(x => x.WidgetId);

                entity
                    .HasMany(x => x.Tags)
                    .WithOne(x => x.Widget)
                    .HasForeignKey(x => x.WidgetId);
            });

            builder.Entity<NewsWidgetCategory>(entity => {
                entity.ToTable("NewsWidgetCategory", "plugin").HasKey(e => e.Id);

                entity.HasOne(x => x.Category).WithMany(x => x.NewsWidgetMap);
            });

            builder.Entity<NewsWidgetTag>(entity => {
                entity.ToTable("NewsWidgetTag", "plugin").HasKey(e => e.Id);
            });
        }
    }
}
