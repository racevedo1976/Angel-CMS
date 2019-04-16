using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Migrations;

namespace Angelo.Connect.Announcement.Data
{
    public class P310005_AddIsPrivateColumn : IAppMigration
    {
        private AnnouncementDbContext _announcementDbContext;

        public string Id { get; } = "P310005";

        public string Migration { get; } = "Add Is Private Column";

        public P310005_AddIsPrivateColumn(AnnouncementDbContext announcementDbContext)
        {
            _announcementDbContext = announcementDbContext;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            // Fail if cannot connect to db
            if (_announcementDbContext.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            // Skip if Tenant.Key already exists
            if (_announcementDbContext.Database.HasColumn("AnnouncementPost", "plugin", "IsPrivate"))
                return MigrationResult.Skipped("Column AnnouncementPost.IsPrivate already exists.");

            // create column initially and allow null
            await _announcementDbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[AnnouncementPost] ADD [IsPrivate] BIT NULL");

            // update the table, set all values to 0
            await _announcementDbContext.Database.ExecuteNonQueryAsync("UPDATE [plugin].[AnnouncementPost] SET IsPrivate = 0");

            // add the not null constraint
            await _announcementDbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[Announcementpost] ALTER COLUMN IsPrivate bit NOT NULL");
            
            return MigrationResult.Success("Added column [AnnouncementPost].[IsPrivate]");
        }
    }
}
