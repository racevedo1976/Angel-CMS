using Angelo.Connect.SlideShow.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.SlideShow.Data
{
    public class GalleryDbContext : DbContext
    {   
        #region Tables
        public DbSet<GalleryWidget> Widgets { get; set; }
        //public DbSet<Models.SlideShow> SlideShows { get; set; }
        public DbSet<GalleryItem> GalleryItems { get; set; }
        #endregion // Tables

        public GalleryDbContext(DbContextOptions<GalleryDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ConfigureWidgetModels(builder);
            //ConfigureSlideModels(builder);
            //ConfigureSlideLayerModels(builder);
        }

        private void ConfigureWidgetModels(ModelBuilder builder)
        {
            builder.Entity<GalleryWidget>(entity =>
            {
                entity.ToTable("GalleryWidget", "plugin").HasKey(e => e.Id);
            });

            builder.Entity<GalleryItem>(entity =>
            {
                entity.ToTable("GalleryItem", "plugin").HasKey(e => e.Id);
            });
        }
    }
}
