using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Migrations;

namespace Angelo.Connect.Blog.Data
{
    public class P410015_AddPublishedColumn : IAppMigration
    {
        private BlogDbContext _blogDbContext;

        public string Id { get; } = "P410015";

        public string Migration { get; } = "Add Published column to BlogPost";

        public P410015_AddPublishedColumn(BlogDbContext blogDbContext)
        {
            _blogDbContext = blogDbContext;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            // Fail if cannot connect to db
            if (_blogDbContext.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            // Skip if column already exists
            if (_blogDbContext.Database.HasColumn("BlogPost", "plugin", "Published"))
                return MigrationResult.Skipped("Column already exists.");

            // create column initially and allow null
            await _blogDbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[BlogPost] ADD [Published] BIT NULL");

            // update the table, set default value
            await _blogDbContext.Database.ExecuteNonQueryAsync("UPDATE [plugin].[BlogPost] SET [Published] = 1");

            // add the not null constraint
            await _blogDbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[BlogPost] ALTER COLUMN [Published] BIT NOT NULL");
            
            return MigrationResult.Success("Added column [BlogPost].[Published]");
        }
    }
}
