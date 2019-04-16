using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Migrations;

namespace Angelo.Connect.Announcement.Data
{
    public class P310015_AddPublishedColumn : IAppMigration
    {
        private AnnouncementDbContext _announcementDbContext;

        public string Id { get; } = "P310015";

        public string Migration { get; } = "Add Published column to AnnouncementPost";

        public P310015_AddPublishedColumn(AnnouncementDbContext announcementDbContext)
        {
            _announcementDbContext = announcementDbContext;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            // Fail if cannot connect to db
            if (_announcementDbContext.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            // Skip if column already exists
            if (_announcementDbContext.Database.HasColumn("AnnouncementPost", "plugin", "Published"))
                return MigrationResult.Skipped("Column already exists.");

            // create column initially and allow null
            await _announcementDbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[AnnouncementPost] ADD [Published] BIT NULL");

            // update the table, set default value
            await _announcementDbContext.Database.ExecuteNonQueryAsync("UPDATE [plugin].[AnnouncementPost] SET [Published] = 1");

            // add the not null constraint
            await _announcementDbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[AnnouncementPost] ALTER COLUMN [Published] BIT NOT NULL");
            
            return MigrationResult.Success("Added column [AnnouncementPost].[Published]");
        }
    }
}
