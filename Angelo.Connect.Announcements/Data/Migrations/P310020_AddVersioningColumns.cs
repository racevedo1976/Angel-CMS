using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Migrations;

namespace Angelo.Connect.Announcement.Data
{
    public class P310020_AddVersioningColumns : IAppMigration
    {
        private AnnouncementDbContext _announcementDbContext;

        public string Id { get; } = "P310020";

        public string Migration { get; } = "Add Versioning columns to AnnouncementPost";

        public P310020_AddVersioningColumns(AnnouncementDbContext announcementDbContext)
        {
            _announcementDbContext = announcementDbContext;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            // Fail if cannot connect to db
            if (_announcementDbContext.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            // Skip if column already exists
            if (_announcementDbContext.Database.HasColumn("AnnouncementPost", "plugin", "VersionCode"))
                return MigrationResult.Skipped("Versioning columns already exists.");

            // drop documentid column
            await _announcementDbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[AnnouncementPost] DROP COLUMN [DocumentId]");
         
            // create column initially and allow null
            await _announcementDbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[AnnouncementPost] ADD [VersionCode] NVARCHAR(50) NULL");
            await _announcementDbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[AnnouncementPost] ADD [Status] INT NULL");


            // update the value of the status column
            await _announcementDbContext.Database.ExecuteNonQueryAsync("UPDATE [plugin].[AnnouncementPost] SET [Status] = 1 WHERE [Published] = 0");
            await _announcementDbContext.Database.ExecuteNonQueryAsync("UPDATE [plugin].[AnnouncementPost] SET [Status] = 2 WHERE [Published] = 1");


            // update values for version code - tricky
            await _announcementDbContext.Database.ExecuteNonQueryAsync(@"
                UPDATE plugin.AnnouncementPost SET VersionCode = ct.VersionCode 
                FROM cms.ContentTree ct 
                WHERE ContentTreeId = ct.Id"
            );
            await _announcementDbContext.Database.ExecuteNonQueryAsync(@"
                UPDATE plugin.AnnouncementPost SET VersionCode = 
                    SUBSTRING(REPLACE(REPLACE(REPLACE(REPLACE(CONVERT(NVARCHAR, Posted, 121), '-', ''), ':', ''), '.', ''), ' ', '-'), 1, 17)
                WHERE VersionCode IS NULL
            ");
            await _announcementDbContext.Database.ExecuteNonQueryAsync(@"
                UPDATE cms.ContentTree SET VersionCode = bp.VersionCode
                FROM plugin.AnnouncementPost bp
                WHERE cms.ContentTree.Id = bp.ContentTreeId
            ");

            // add the not null constraint
            await _announcementDbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[AnnouncementPost] ALTER COLUMN [VersionCode] NVARCHAR(50) NOT NULL");
            await _announcementDbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[AnnouncementPost] ALTER COLUMN [Status] INT NOT NULL");

            return MigrationResult.Success("Added [AnnouncementPost].[VersionCode], Added [AnnouncementPost].[Status], Removed [AnnouncementPost].[DocumentId]");
        }
    }
}
