using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Migrations;

namespace Angelo.Connect.News.Data
{
    public class P910006_AddActiveColumn : IAppMigration
    {
        private NewsDbContext _NewsDbContext;

        public string Id { get; } = "P910006";

        public string Migration { get; } = "Add Is Active Column";

        public P910006_AddActiveColumn(NewsDbContext newsDbContext)
        {
            _NewsDbContext = newsDbContext;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            // Fail if cannot connect to db
            if (_NewsDbContext.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            // Skip if Tenant.Key already exists
            if (_NewsDbContext.Database.HasColumn("NewsPost", "plugin", "IsActive"))
                return MigrationResult.Skipped("Column NewsPost.IsActive already exists.");

            // create column initially and allow null
            await _NewsDbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[NewsPost] ADD [IsActive] BIT NULL DEFAULT (1)");

            // update the table, set all values to 0
            await _NewsDbContext.Database.ExecuteNonQueryAsync("UPDATE [plugin].[NewsPost] SET IsActive = 1");

            // add the not null constraint
            await _NewsDbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[Newspost] ALTER COLUMN IsActive bit NOT NULL");
            
            return MigrationResult.Success("Added column [NewsPost].[IsActive]");
        }
    }
}
