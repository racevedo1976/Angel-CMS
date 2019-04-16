using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Migrations;

namespace Angelo.Connect.Blog.Data
{
    public class P410006_AddActiveColumn : IAppMigration
    {
        private BlogDbContext _blogDbContext;

        public string Id { get; } = "P410006";

        public string Migration { get; } = "Add Is Active Column";

        public P410006_AddActiveColumn(BlogDbContext blogDbContext)
        {
            _blogDbContext = blogDbContext;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            // Fail if cannot connect to db
            if (_blogDbContext.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            // Skip if Tenant.Key already exists
            if (_blogDbContext.Database.HasColumn("BlogPost", "plugin", "IsActive"))
                return MigrationResult.Skipped("Column BlogPost.IsActive already exists.");

            // create column initially and allow null
            await _blogDbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[BlogPost] ADD [IsActive] BIT NULL DEFAULT (1)");

            // update the table, set all values to 0
            await _blogDbContext.Database.ExecuteNonQueryAsync("UPDATE [plugin].[BlogPost] SET IsActive = 1");

            // add the not null constraint
            await _blogDbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[Blogpost] ALTER COLUMN IsActive bit NOT NULL");
            
            return MigrationResult.Success("Added column [BlogPost].[IsActive]");
        }
    }
}
