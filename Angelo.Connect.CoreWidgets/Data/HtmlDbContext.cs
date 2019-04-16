using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Infrastructure;

using Angelo.Connect.CoreWidgets.Models;

namespace Angelo.Connect.CoreWidgets.Data
{
    public partial class HtmlDbContext : DbContext
    { 
        public DbSet<NavBar> NavBars { get; set; }

        public HtmlDbContext(DbContextOptions<HtmlDbContext> options) : base(options)
        {   
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<NavBar>(entity => {
                entity
                    .ToTable("NavBar", "plugin")
                    .HasKey(e => e.Id);

                entity.Ignore(x => x.NavMenu);
            });
        }
    }
}
