using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Angelo.Connect.Assignments.Models;

namespace Angelo.Connect.Assignments.Data
{
    public class AssignmentsDbContext : DbContext
    {
        public DbSet<AssignmentWidget> AssignmentWidgets { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<AssignmentCategory> AssignmentCategories { get; set; }
        public DbSet<AssignmentCategoryLink> AssignmentCategoryLinks { get; set; }
        public DbSet<AssignmentUserGroup> AssignmentUserGroups { get; set; }


        public AssignmentsDbContext(DbContextOptions<AssignmentsDbContext> options) : base(options)
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
            builder.Entity<AssignmentWidget>(entity =>
            {
                entity.ToTable("AssignmentWidget", "plugin").HasKey(e => e.Id);
            });

            builder.Entity<Assignment>(n =>
            {
                n.ToTable("Assignment", "plugin").HasKey(x => x.Id);
                n.Property(x => x.Status).HasMaxLength(10);
            });

            builder.Entity<AssignmentUserGroup>(g =>
            {
                g.ToTable("AssignmentUserGroup", "plugin").HasKey(x => new { x.AssignmentId, x.UserGroupId });
            });

            //builder.Entity<AssignmentCategory>(l =>
            //{
            //    l.ToTable("AssignmentCategory", "plugin").HasKey(x => x.Id);
            //});

            builder.Entity<AssignmentCategory>()
                .ToTable("AssignmentCategory", "plugin")
                .HasKey(x => x.Id);

            builder.Entity<AssignmentCategoryLink>(l =>
            {
                l.ToTable("AssignmentCategoryLink", "plugin").HasKey(x => new { x.AssignmentId, x.CategoryId });
            });
        }
    }
}
