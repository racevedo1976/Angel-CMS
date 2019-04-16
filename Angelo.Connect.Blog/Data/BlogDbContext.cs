using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Blog.Models;

namespace Angelo.Connect.Blog.Data
{
    public class BlogDbContext : DbContext
    {
        public DbSet<BlogCategory> BlogCategories { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<BlogPostCategory> BlogPostCategories { get; set; }
        public DbSet<BlogPostTag> BlogPostTags { get; set; }
        public DbSet<BlogWidget> BlogWidgets { get; set; }
        public DbSet<BlogWidgetCategory> BlogWidgetCategories { get; set; }
        public DbSet<BlogWidgetTag> BlogWidgetTags { get; set; }

        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureBlogModels(modelBuilder);
            ConfigureWidgetModels(modelBuilder);
        }

        private static void ConfigureBlogModels(ModelBuilder builder)
        {
            builder.Entity<BlogCategory>(entity => {
                entity.ToTable("BlogCategory", "plugin").HasKey(e => e.Id);

                entity.HasMany(x => x.BlogPostMap).WithOne(x => x.Category);
            });

            builder.Entity<BlogPost>(entity => {
                entity.ToTable("BlogPost", "plugin").HasKey(e => e.Id);

                entity
                    .HasMany(x => x.Categories)
                    .WithOne(x => x.Post)
                    .HasForeignKey(x => x.PostId);

                entity
                    .HasMany(x => x.Tags)
                    .WithOne(x => x.Post)
                    .HasForeignKey(x => x.PostId);
            });

            builder.Entity<BlogPostCategory>(entity => {
                entity.ToTable("BlogPostCategory", "plugin").HasKey(e => e.Id);
            });

            builder.Entity<BlogPostTag>(entity => {
                entity.ToTable("BlogPostTag", "plugin").HasKey(e => e.Id);
            });
        }

        private static void ConfigureWidgetModels(ModelBuilder builder)
        {         
            builder.Entity<BlogWidget>(entity => {
                entity.ToTable("BlogWidget", "plugin").HasKey(e => e.Id);
             
                entity
                    .HasMany(x => x.Categories)
                    .WithOne(x => x.Widget)
                    .HasForeignKey(x => x.WidgetId);

                entity
                    .HasMany(x => x.Tags)
                    .WithOne(x => x.Widget)
                    .HasForeignKey(x => x.WidgetId);
            });

            builder.Entity<BlogWidgetCategory>(entity => {
                entity.ToTable("BlogWidgetCategory", "plugin").HasKey(e => e.Id);

                entity.HasOne(x => x.Category).WithMany(x => x.BlogWidgetMap);
            });

            builder.Entity<BlogWidgetTag>(entity => {
                entity.ToTable("BlogWidgetTag", "plugin").HasKey(e => e.Id);
            });
        }
    }
}
