using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Migrations;

namespace Angelo.Connect.News.Data
{
    public class P910015_AddPublishedColumn : IAppMigration
    {
        private NewsDbContext _NewsDbContext;

        public string Id { get; } = "P910015";

        public string Migration { get; } = "Add Published column to NewsPost";

        public P910015_AddPublishedColumn(NewsDbContext newsDbContext)
        {
            _NewsDbContext = newsDbContext;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            // Fail if cannot connect to db
            if (_NewsDbContext.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            // Skip if column already exists
            if (_NewsDbContext.Database.HasColumn("NewsPost", "plugin", "Published"))
                return MigrationResult.Skipped("Column already exists.");

            // create column initially and allow null
            await _NewsDbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[NewsPost] ADD [Published] BIT NULL");

            // update the table, set default value
            await _NewsDbContext.Database.ExecuteNonQueryAsync("UPDATE [plugin].[NewsPost] SET [Published] = 1");

            // add the not null constraint
            await _NewsDbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[NewsPost] ALTER COLUMN [Published] BIT NOT NULL");
            
            return MigrationResult.Success("Added column [NewsPost].[Published]");
        }
    }
}
