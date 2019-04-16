using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Migrations;

namespace Angelo.Connect.News.Data
{
    public class P910020_AddVersioningColumns : IAppMigration
    {
        private NewsDbContext _NewsDbContext;

        public string Id { get; } = "P910020";

        public string Migration { get; } = "Add Versioning columns to NewsPost";

        public P910020_AddVersioningColumns(NewsDbContext newsDbContext)
        {
            _NewsDbContext = newsDbContext;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            // Fail if cannot connect to db
            if (_NewsDbContext.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            // Skip if column already exists
            if (_NewsDbContext.Database.HasColumn("NewsPost", "plugin", "VersionCode"))
                return MigrationResult.Skipped("Versioning columns already exists.");

            // drop documentid column
            await _NewsDbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[NewsPost] DROP COLUMN [DocumentId]");
         
            // create column initially and allow null
            await _NewsDbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[NewsPost] ADD [VersionCode] NVARCHAR(50) NULL");
            await _NewsDbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[NewsPost] ADD [Status] INT NULL");


            // update the value of the status column
            await _NewsDbContext.Database.ExecuteNonQueryAsync("UPDATE [plugin].[NewsPost] SET [Status] = 1 WHERE [Published] = 0");
            await _NewsDbContext.Database.ExecuteNonQueryAsync("UPDATE [plugin].[NewsPost] SET [Status] = 2 WHERE [Published] = 1");


            // update values for version code - tricky
            await _NewsDbContext.Database.ExecuteNonQueryAsync(@"
                UPDATE plugin.NewsPost SET VersionCode = ct.VersionCode 
                FROM cms.ContentTree ct 
                WHERE ContentTreeId = ct.Id"
            );
            await _NewsDbContext.Database.ExecuteNonQueryAsync(@"
                UPDATE plugin.NewsPost SET VersionCode = 
                    SUBSTRING(REPLACE(REPLACE(REPLACE(REPLACE(CONVERT(NVARCHAR, Posted, 121), '-', ''), ':', ''), '.', ''), ' ', '-'), 1, 17)
                WHERE VersionCode IS NULL
            ");
            await _NewsDbContext.Database.ExecuteNonQueryAsync(@"
                UPDATE cms.ContentTree SET VersionCode = bp.VersionCode
                FROM plugin.NewsPost bp
                WHERE cms.ContentTree.Id = bp.ContentTreeId
            ");

            // add the not null constraint
            await _NewsDbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[NewsPost] ALTER COLUMN [VersionCode] NVARCHAR(50) NOT NULL");
            await _NewsDbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[NewsPost] ALTER COLUMN [Status] INT NOT NULL");

            return MigrationResult.Success("Added [NewsPost].[VersionCode], Added [NewsPost].[Status], Removed [NewsPost].[DocumentId]");
        }
    }
}
