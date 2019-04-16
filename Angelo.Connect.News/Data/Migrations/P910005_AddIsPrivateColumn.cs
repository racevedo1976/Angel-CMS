using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Migrations;

namespace Angelo.Connect.News.Data
{
    public class P910005_AddIsPrivateColumn : IAppMigration
    {
        private NewsDbContext _NewsDbContext;

        public string Id { get; } = "P910005";

        public string Migration { get; } = "Add Is Private Column";

        public P910005_AddIsPrivateColumn(NewsDbContext newsDbContext)
        {
            _NewsDbContext = newsDbContext;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            // Fail if cannot connect to db
            if (_NewsDbContext.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            // Skip if Tenant.Key already exists
            if (_NewsDbContext.Database.HasColumn("NewsPost", "plugin", "IsPrivate"))
                return MigrationResult.Skipped("Column NewsPost.IsPrivate already exists.");

            // create column initially and allow null
            await _NewsDbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[NewsPost] ADD [IsPrivate] BIT NULL");

            // update the table, set all values to 0
            await _NewsDbContext.Database.ExecuteNonQueryAsync("UPDATE [plugin].[NewsPost] SET IsPrivate = 0");

            // add the not null constraint
            await _NewsDbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[Newspost] ALTER COLUMN IsPrivate bit NOT NULL");
            
            return MigrationResult.Success("Added column [NewsPost].[IsPrivate]");
        }
    }
}
