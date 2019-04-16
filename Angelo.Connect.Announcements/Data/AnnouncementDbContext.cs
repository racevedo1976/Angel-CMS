using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Announcement.Models;

namespace Angelo.Connect.Announcement.Data
{
    public class AnnouncementDbContext : DbContext
    {
        public DbSet<AnnouncementCategory> AnnouncementCategories { get; set; }
        public DbSet<AnnouncementPost> AnnouncementPosts { get; set; }
        public DbSet<AnnouncementPostCategory> AnnouncementPostCategories { get; set; }
        public DbSet<AnnouncementPostTag> AnnouncementPostTags { get; set; }
        public DbSet<AnnouncementWidget> AnnouncementWidgets { get; set; }
        public DbSet<AnnouncementWidgetCategory> AnnouncementWidgetCategories { get; set; }
        public DbSet<AnnouncementWidgetTag> AnnouncementWidgetTags { get; set; }

        public AnnouncementDbContext(DbContextOptions<AnnouncementDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureAnnouncementModels(modelBuilder);
            ConfigureWidgetModels(modelBuilder);
        }

        private static void ConfigureAnnouncementModels(ModelBuilder builder)
        {
            builder.Entity<AnnouncementCategory>(entity => {
                entity.ToTable("AnnouncementCategory", "plugin").HasKey(e => e.Id);

                entity.HasMany(x => x.AnnouncementPostMap).WithOne(x => x.Category);
            });

            builder.Entity<AnnouncementPost>(entity => {
                entity.ToTable("AnnouncementPost", "plugin").HasKey(e => e.Id);

                entity
                    .HasMany(x => x.Categories)
                    .WithOne(x => x.Post)
                    .HasForeignKey(x => x.PostId);

                entity
                    .HasMany(x => x.Tags)
                    .WithOne(x => x.Post)
                    .HasForeignKey(x => x.PostId);
            });

            builder.Entity<AnnouncementPostCategory>(entity => {
                entity.ToTable("AnnouncementPostCategory", "plugin").HasKey(e => e.Id);
            });

            builder.Entity<AnnouncementPostTag>(entity => {
                entity.ToTable("AnnouncementPostTag", "plugin").HasKey(e => e.Id);
            });
        }

        private static void ConfigureWidgetModels(ModelBuilder builder)
        {         
            builder.Entity<AnnouncementWidget>(entity => {
                entity.ToTable("AnnouncementWidget", "plugin").HasKey(e => e.Id);
             
                entity
                    .HasMany(x => x.Categories)
                    .WithOne(x => x.Widget)
                    .HasForeignKey(x => x.WidgetId);

                entity
                    .HasMany(x => x.Tags)
                    .WithOne(x => x.Widget)
                    .HasForeignKey(x => x.WidgetId);
            });

            builder.Entity<AnnouncementWidgetCategory>(entity => {
                entity.ToTable("AnnouncementWidgetCategory", "plugin").HasKey(e => e.Id);

                entity.HasOne(x => x.Category).WithMany(x => x.AnnouncementWidgetMap);
            });

            builder.Entity<AnnouncementWidgetTag>(entity => {
                entity.ToTable("AnnouncementWidgetTag", "plugin").HasKey(e => e.Id);
            });
        }
    }
}
