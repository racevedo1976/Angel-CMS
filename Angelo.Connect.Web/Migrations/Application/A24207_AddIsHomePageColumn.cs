using System.Linq;
using System.Threading.Tasks;
using Angelo.Common.Migrations;
using Angelo.Connect.Data;
using Microsoft.EntityFrameworkCore;

namespace Angelo.Connect.Web.Migrations.Application
{
    public class A24207_AddIsHomePageColumn : IAppMigration
    {
        private readonly ConnectDbContext _connectDb;

        public string Id { get; } = "A24207";

        public string Migration { get; } = "Add Column IsHomePage to Pages";

        public A24207_AddIsHomePageColumn(ConnectDbContext connectDb)
        {
            _connectDb = connectDb;
        }
        public async Task<MigrationResult> ExecuteAsync()
        {
            // Fail if cannot connect to db
            if (_connectDb.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            // Add the new column
            await _connectDb.Database.ExecuteNonQueryAsync("ALTER TABLE [cms].[Page] ADD [IsHomePage] BIT NOT NULL DEFAULT(0)");

            await UpdateFlagOnExistingSites();

            return MigrationResult.Success("Successfully added Column IsHomePage to Pages.");
        }

        private async Task<bool> UpdateFlagOnExistingSites()
        {
          
            var homePages = _connectDb.Pages.Where(x => x.Path == "/").ToList();

            if (homePages.Any())
            {
                foreach (var page in homePages)
                {
                    page.IsHomePage = true;
                    _connectDb.Pages.Update(page);
                }

                await _connectDb.SaveChangesAsync();
            }

            return await Task.FromResult(true);

        }
    }
}
