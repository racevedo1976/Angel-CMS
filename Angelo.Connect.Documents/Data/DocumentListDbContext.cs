using Microsoft.EntityFrameworkCore;
using Angelo.Connect.Documents.Models;

namespace Angelo.Connect.Documents.Data
{
    public class DocumentListDbContext : DbContext
    {
        public DbSet<DocumentListWidget> DocumentListWidgets { get; set; }

        public DbSet<DocumentListDocument> DocumentListDocuments { get; set; }
        public DbSet<DocumentListFolder> DocumentListFolders { get; set; }

        public DocumentListDbContext(DbContextOptions<DocumentListDbContext> options) : base(options)
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
            builder.Entity<DocumentListWidget>(entity =>
            {
                entity.ToTable("DocumentListWidget", "plugin").HasKey(e => e.Id);
                entity
                    .HasMany(x => x.Documents)
                    .WithOne(x => x.Widget)
                    .HasForeignKey(x => x.WidgetId);
                entity
                    .HasMany(x => x.Folders)
                    .WithOne(x => x.Widget)
                    .HasForeignKey(x => x.WidgetId);
            });

            builder.Entity<DocumentListDocument>(entity =>
            {
                entity.ToTable("DocumentListDocument", "plugin").HasKey(e => e.Id);
            });

            builder.Entity<DocumentListFolder>(entity =>
            {
                entity.ToTable("DocumentListFolder", "plugin").HasKey(e => e.Id);
                entity
                    .HasMany(x => x.Documents).WithOne(x => x.Folder).HasForeignKey(x => x.FolderId);
            });
        }
    }
}
