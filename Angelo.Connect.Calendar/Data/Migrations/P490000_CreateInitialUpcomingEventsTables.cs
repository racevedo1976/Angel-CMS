using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Common.Migrations;
using Angelo.Connect.Calendar.Data;
using Microsoft.EntityFrameworkCore;

namespace Angelo.Connect.Calendar.Data
{
    public class P490000_CreateInitialUpcomingEventsTables : IAppMigration
    {
        public string Id { get; } = "P490000";

        public string Migration { get; } = "Create Initial Upcoming Events Tables";

        private CalendarDbContext _dbContext;

        public P490000_CreateInitialUpcomingEventsTables(CalendarDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            if (_dbContext.Database.TableExists("UpcomingEventsWidget", "plugin"))
                return MigrationResult.Skipped();


            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                    CREATE TABLE [plugin].[UpcomingEventsWidget](
	                    [Id] [nvarchar](50) NOT NULL,
                        [Title] [nvarchar](500) NULL,
                        [PostsToDisplay] int NOT NULL,
                        [UseTextColor] bit NOT NULL DEFAULT 0,
                        [TextColor] [nvarchar](10) NULL,
                        CONSTRAINT [PK_UpcomingEventsWidget] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )
                ");

            if (_dbContext.Database.TableExists("UpcomingEventsWidgetEventGroup", "plugin"))
                return MigrationResult.Skipped();

            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                    CREATE TABLE [plugin].[UpcomingEventsWidgetEventGroup](
                        [Id] [nvarchar](50) NOT NULL,
                        [WidgetId] [nvarchar](50) NOT NULL,
                        [EventGroupId] [nvarchar](50) NOT NULL,
                        CONSTRAINT [PK_UpcomingEventsWidgetEventGroup] PRIMARY KEY CLUSTERED ([WidgetId] ASC, [EventGroupId] ASC)
                    )
                ");

            _dbContext.Database.ExecuteNonQuery("ALTER TABLE [plugin].[UpcomingEventsWidgetEventGroup] ADD CONSTRAINT [AK_Id] UNIQUE NONCLUSTERED ([Id] ASC)");


            return MigrationResult.Success();
        }
    }
}
