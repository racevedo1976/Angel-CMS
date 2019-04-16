using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Migrations;

namespace Angelo.Connect.Blog.Data
{
    public class P410005_AddIsPrivateColumn : IAppMigration
    {
        private BlogDbContext _blogDbContext;

        public string Id { get; } = "P410005";

        public string Migration { get; } = "Add Is Private Column";

        public P410005_AddIsPrivateColumn(BlogDbContext blogDbContext)
        {
            _blogDbContext = blogDbContext;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            // Fail if cannot connect to db
            if (_blogDbContext.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            // Skip if Tenant.Key already exists
            if (_blogDbContext.Database.HasColumn("BlogPost", "plugin", "IsPrivate"))
                return MigrationResult.Skipped("Column BlogPost.IsPrivate already exists.");

            // create column initially and allow null
            await _blogDbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[BlogPost] ADD [IsPrivate] BIT NULL");

            // update the table, set all values to 0
            await _blogDbContext.Database.ExecuteNonQueryAsync("UPDATE [plugin].[BlogPost] SET IsPrivate = 0");

            // add the not null constraint
            await _blogDbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[Blogpost] ALTER COLUMN IsPrivate bit NOT NULL");
            
            return MigrationResult.Success("Added column [BlogPost].[IsPrivate]");
        }
    }
}
