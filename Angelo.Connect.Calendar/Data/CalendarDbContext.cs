using Angelo.Connect.Calendar.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Calendar.Data
{
    public class CalendarDbContext : DbContext
    {
        public CalendarDbContext(DbContextOptions<CalendarDbContext> options) : base(options)
        {
        }

        public DbSet<CalendarEvent> CalendarEvents { get; set; }
        public DbSet<CalendarWidgetSetting> CalendarWidgetSettings  { get; set; }
        public DbSet<CalendarEventGroup> CalendarEventGroups { get; set; }
        public DbSet<CalendarEventTag> CalendarEventTags { get; set; }
        public DbSet<CalendarWidgetEventGroup> CalendarWidgetEventGroups { get; set; }
        public DbSet<CalendarEventRecurrence> CalendarEventRecurrences { get; set; }
        public DbSet<UpcomingEventsWidget> UpcomingEventsWidget { get; set; }
        public DbSet<UpcomingEventsWidgetEventGroup> UpcomingEventsWidgetEventGroups { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureCalendarModels(modelBuilder);
            ConfigureWidgetModels(modelBuilder);
        }

        private void ConfigureWidgetModels(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CalendarWidgetSetting>(entity => {
                entity.ToTable("CalendarWidgetSetting", "plugin").HasKey(e => e.Id);
            });

            modelBuilder.Entity<UpcomingEventsWidget>(entity => {
                entity.ToTable("UpcomingEventsWidget", "plugin").HasKey(e => e.Id);
            });

            modelBuilder.Entity<UpcomingEventsWidgetEventGroup>(entity =>
            {
                entity.ToTable("UpcomingEventsWidgetEventGroup", "plugin").HasKey(e => new { e.WidgetId, e.EventGroupId });
                entity.HasAlternateKey(e => e.Id);
            });

        }

        private static void ConfigureCalendarModels(ModelBuilder builder)
        {

            builder.Entity<CalendarEvent>(entity =>
            {
                entity.ToTable("CalendarEvent", "plugin").HasKey(e => e.EventId);
                entity.HasOne(re => re.RecurrenceDetails).WithOne(r => r.Event).HasForeignKey<CalendarEventRecurrence>(b => b.EventId);
            })
            
            ;

            builder.Entity<CalendarEventGroup>(entity =>
           {
               entity.ToTable("CalendarEventGroup", "plugin").HasKey(e => e.EventGroupId);
           });

            builder.Entity<CalendarEventGroupEvent>(entity =>
            {
                entity.ToTable("CalendarEventGroupEvent", "plugin").HasKey(t => new { t.EventId, t.EventGroupId });

                entity.HasOne(e => e.Event).WithMany(ev => ev.EventGroupEvents).HasForeignKey(ev => ev.EventId);

                //entity.HasOne(e => e.EventGroup).WithMany(eg => eg.EventGroupEvents).HasForeignKey(eg => eg.EventGroupId);
            });



            builder.Entity<CalendarWidgetEventGroup>(entity =>
            {
                entity.ToTable("CalendarWidgetEventGroup", "plugin").HasKey(e => e.Id);
                //entity.HasOne(e => e.EventGroup).WithMany(eg => eg.WidgetGroups).HasForeignKey(eg => eg.EventGroupId);
            });

            builder.Entity<CalendarEventTag>(entity =>
            {
                entity.ToTable("CalendarEventTag", "plugin").HasKey(e => e.Id);
            });

            builder.Entity<CalendarEventRecurrence>(entity =>
            {
                entity.ToTable("CalendarEventRecurrence", "plugin").HasKey(e => e.EventId);
                
            });

        }
    }
}
