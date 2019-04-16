using Angelo.Connect.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Angelo.Connect.Logging
{
    public class DbLogContext : DbContext
    {
        public DbSet<DbLogEvent> Events { get; set; }
        public DbSet<DocumentEvent> DocumentEvents { get; set; }
        public DbLogContext(DbContextOptions<DbLogContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<DbLogEvent>(entity => {
                entity.ToTable("LogEvent", "log");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).UseSqlServerIdentityColumn();
            });
            builder.Entity<DocumentEvent>(entity => {
                entity.ToTable("DocumentEvent", "log");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).UseSqlServerIdentityColumn();
            });
        }
    }
}
