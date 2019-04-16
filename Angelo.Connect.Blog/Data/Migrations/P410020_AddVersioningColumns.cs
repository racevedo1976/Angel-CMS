using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Migrations;

namespace Angelo.Connect.Blog.Data
{
    public class P410020_AddVersioningColumns : IAppMigration
    {
        private BlogDbContext _blogDbContext;

        public string Id { get; } = "P410020";

        public string Migration { get; } = "Add Versioning columns to BlogPost";

        public P410020_AddVersioningColumns(BlogDbContext blogDbContext)
        {
            _blogDbContext = blogDbContext;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            // Fail if cannot connect to db
            if (_blogDbContext.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            // Skip if column already exists
            if (_blogDbContext.Database.HasColumn("BlogPost", "plugin", "VersionCode"))
                return MigrationResult.Skipped("Versioning columns already exists.");

            // drop documentid column
            await _blogDbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[BlogPost] DROP COLUMN [DocumentId]");
         
            // create column initially and allow null
            await _blogDbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[BlogPost] ADD [VersionCode] NVARCHAR(50) NULL");
            await _blogDbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[BlogPost] ADD [Status] INT NULL");


            // update the value of the status column
            await _blogDbContext.Database.ExecuteNonQueryAsync("UPDATE [plugin].[BlogPost] SET [Status] = 1 WHERE [Published] = 0");
            await _blogDbContext.Database.ExecuteNonQueryAsync("UPDATE [plugin].[BlogPost] SET [Status] = 2 WHERE [Published] = 1");


            // update values for version code - tricky
            await _blogDbContext.Database.ExecuteNonQueryAsync(@"
                UPDATE plugin.BlogPost SET VersionCode = ct.VersionCode 
                FROM cms.ContentTree ct 
                WHERE ContentTreeId = ct.Id"
            );
            await _blogDbContext.Database.ExecuteNonQueryAsync(@"
                UPDATE plugin.BlogPost SET VersionCode = 
                    SUBSTRING(REPLACE(REPLACE(REPLACE(REPLACE(CONVERT(NVARCHAR, Posted, 121), '-', ''), ':', ''), '.', ''), ' ', '-'), 1, 17)
                WHERE VersionCode IS NULL
            ");
            await _blogDbContext.Database.ExecuteNonQueryAsync(@"
                UPDATE cms.ContentTree SET VersionCode = bp.VersionCode
                FROM plugin.BlogPost bp
                WHERE cms.ContentTree.Id = bp.ContentTreeId
            ");

            // add the not null constraint
            await _blogDbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[BlogPost] ALTER COLUMN [VersionCode] NVARCHAR(50) NOT NULL");
            await _blogDbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[BlogPost] ALTER COLUMN [Status] INT NOT NULL");

            return MigrationResult.Success("Added [BlogPost].[VersionCode], Added [BlogPost].[Status], Removed [BlogPost].[DocumentId]");
        }
    }
}
