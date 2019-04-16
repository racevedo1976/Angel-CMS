using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Migrations;
using System.Threading.Tasks;

namespace Angelo.Connect.Calendar.Data
{
    public class P480006_AddAvailableViews : IAppMigration
    {
        private CalendarDbContext _dbContext;

        public string Id { get; } = "P480006";

        public string Migration { get; } = "Add available views";

        public P480006_AddAvailableViews(CalendarDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {

           
            // Fail if cannot connect to db
            if (_dbContext.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            // Skip if MonthView already exists
            if (_dbContext.Database.HasColumn("CalendarWidgetSetting", "plugin", "MonthView"))
                return MigrationResult.Skipped("Column BlogPost.IsPrivate already exists.");

            // 
            await _dbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[CalendarWidgetSetting] ADD [MonthView] BIT NOT NULL DEFAULT(1)");
            await _dbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[CalendarWidgetSetting] ADD [WeekView] BIT NOT NULL DEFAULT(1)");
            await _dbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[CalendarWidgetSetting] ADD [DayView] BIT NOT NULL DEFAULT(1)");
            await _dbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[CalendarWidgetSetting] ADD [ListView] BIT NOT NULL DEFAULT(1)");
           
            
            return MigrationResult.Success("Successfully added view columns.");
        }
    }
}
