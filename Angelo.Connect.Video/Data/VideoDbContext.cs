using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Video.Models;

namespace Angelo.Connect.Video.Data
{
    public class VideoDbContext : DbContext
    {
        public DbSet<VideoWidget> VideoWidgets { get; set; }
        public DbSet<VideoBackgroundWidget> VideoBackgroundWidgets { get; set; }
        public DbSet<VideoStreamLink> VideoStreamLinks { get; set; }

        public VideoDbContext(DbContextOptions<VideoDbContext> options) : base(options)
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
            builder.Entity<VideoWidget>(entity =>
            {
                entity.ToTable("VideoWidget", "plugin").HasKey(e => e.Id);

                //entity.HasOne(x => x.Source)
                //      .WithMany(x => x.VideoLinks)
                //      .HasForeignKey(x => x.SourceId);
            });
                
            builder.Entity<VideoStreamLink>(entity =>
            {
                entity.ToTable("VideoStreamLink", "plugin").HasKey(e => e.Id);
            });

            builder.Entity<VideoBackgroundWidget>(entity =>
            {
                entity.ToTable("VideoBackgroundWidget", "plugin").HasKey(e => e.Id);
            });
        }
    }
}
