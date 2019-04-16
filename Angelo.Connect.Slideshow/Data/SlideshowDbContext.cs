using Angelo.Connect.SlideShow.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.SlideShow.Data
{
    public class SlideShowDbContext : DbContext
    {   
        #region Tables
        public DbSet<SlideShowWidget> Widgets { get; set; }
        //public DbSet<Models.SlideShow> SlideShows { get; set; }
        public DbSet<Slide> Slides { get; set; }
        public DbSet<KenBurnsEffect> KenBurnsEffects { get; set; }
        public DbSet<Parallax> Parallaxes { get; set; }
        public DbSet<SlideLayer> SlideLayers { get; set; }
        #endregion // Tables

        public SlideShowDbContext(DbContextOptions<SlideShowDbContext> options) : base(options)
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
            ConfigureSlideModels(builder);
            ConfigureSlideLayerModels(builder);
        }

        private void ConfigureWidgetModels(ModelBuilder builder)
        {
            builder.Entity<SlideShowWidget>(entity => {
                entity.ToTable("SlideShowWidget", "plugin").HasKey(e => e.Id);

                // Taking this out, leaving it to be a one-way relationship (widget to show)
                //entity.HasOne<Models.SlideShow>(x => x.SlideShow)
                //    .WithOne(x => x.Widget)
                //    .HasForeignKey<SlideShowWidget>(x => x.SlideShowId);
            });
            //builder.Entity<Models.SlideShow>(entity => {
            //    entity.ToTable("SlideShowLux", "plugin").HasKey(e => e.Id);
            //});
        }

        private static void ConfigureSlideModels(ModelBuilder builder)
        {
            builder.Entity<Slide>(entity => {
                entity.ToTable("SlideShowSlide", "plugin").HasKey(e => e.DocumentId);

                entity
                    .HasMany(x => x.Layers)
                    .WithOne(x => x.Slide)
                    .HasForeignKey(x => x.SlideId);
                entity
                    .HasOne(x => x.KenBurnsEffect)
                    .WithOne(x => x.Slide)
                    .HasForeignKey<KenBurnsEffect>(x => x.SlideId);
                entity
                    .HasOne(x => x.Parallax)
                    .WithOne(x => x.Slide)
                    .HasForeignKey<Parallax>(x => x.SlideId);
            });

            builder.Entity<Parallax>(entity => {
                entity.ToTable("SlideShowParallax", "plugin").HasKey(e => e.Id);
            });

            builder.Entity<KenBurnsEffect>(entity => {
                entity.ToTable("SlideShowKenBurnsEffect", "plugin").HasKey(e => e.Id);
            });
        }

        private static void ConfigureSlideLayerModels(ModelBuilder builder)
        {
            builder.Entity<SlideLayer>(entity =>
            {
                entity.ToTable("SlideShowLayer", "plugin").HasKey(e => e.Id);

                // Ensures that SlideLayer.Slide is populated
                entity.HasOne(x => x.Slide)
                    .WithMany(x => x.Layers)
                    .HasForeignKey(x => x.SlideId);
            });
        }
    }
}
