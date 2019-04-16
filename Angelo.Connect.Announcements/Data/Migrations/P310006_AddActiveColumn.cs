using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Migrations;

namespace Angelo.Connect.Announcement.Data
{
    public class P310006_AddActiveColumn : IAppMigration
    {
        private AnnouncementDbContext _announcementDbContext;

        public string Id { get; } = "P310006";

        public string Migration { get; } = "Add Is Active Column";

        public P310006_AddActiveColumn(AnnouncementDbContext announcementDbContext)
        {
            _announcementDbContext = announcementDbContext;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            // Fail if cannot connect to db
            if (_announcementDbContext.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            // Skip if Tenant.Key already exists
            if (_announcementDbContext.Database.HasColumn("AnnouncementPost", "plugin", "IsActive"))
                return MigrationResult.Skipped("Column AnnouncementPost.IsActive already exists.");

            // create column initially and allow null
            await _announcementDbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[AnnouncementPost] ADD [IsActive] BIT NULL DEFAULT (1)");

            // update the table, set all values to 0
            await _announcementDbContext.Database.ExecuteNonQueryAsync("UPDATE [plugin].[AnnouncementPost] SET IsActive = 1");

            // add the not null constraint
            await _announcementDbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[Announcementpost] ALTER COLUMN IsActive bit NOT NULL");
            
            return MigrationResult.Success("Added column [AnnouncementPost].[IsActive]");
        }
    }
}
