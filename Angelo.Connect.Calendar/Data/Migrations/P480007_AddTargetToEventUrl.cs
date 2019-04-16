using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Migrations;
using System.Threading.Tasks;

namespace Angelo.Connect.Calendar.Data
{
    public class P480007_AddTargetToEventUrl : IAppMigration
    {
        private CalendarDbContext _dbContext;

        public string Id { get; } = "P480007";

        public string Migration { get; } = "Add Target To Event Url";

        public P480007_AddTargetToEventUrl(CalendarDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            //[LinkTarget]
            //    [nvarchar] (7) NOT NULL DEFAULT '_self',
           
            // Fail if cannot connect to db
            if (_dbContext.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            // Skip if MonthView already exists
            if (_dbContext.Database.HasColumn("CalendarEvent", "plugin", "LinkTarget"))
                return MigrationResult.Skipped("Column CalendarEvent.LinkTarget already exists.");

            // 
            await _dbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[CalendarEvent] ADD [LinkTarget] [nvarchar] (7) NOT NULL DEFAULT '_self'");
          
            return MigrationResult.Success("Successfully Add Target To Event Url.");
        }
    }
}
