using System.Threading.Tasks;
using Angelo.Common.Migrations;
using Angelo.Connect.Data;
using Microsoft.EntityFrameworkCore;

namespace Angelo.Connect.Web.Migrations.Application
{
    public class A16030_UpdateNavMenuTargetType : IAppMigration
    {
        public string Id { get; } = "A16030";

        public string Migration { get; } = "Update NavMenu TargetType to bit";

        private ConnectDbContext _connectDb;

        public A16030_UpdateNavMenuTargetType(ConnectDbContext connectDb)
        {
            _connectDb = connectDb;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            // Fail if cannot connect to db
            if (_connectDb.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            await _connectDb.Database.ExecuteNonQueryAsync("ALTER TABLE [cms].[NavigationMenuItem] ALTER COLUMN [TargetType] bit");

            return MigrationResult.Success("Altered [TargetType] to bit");
        }
    }
}
