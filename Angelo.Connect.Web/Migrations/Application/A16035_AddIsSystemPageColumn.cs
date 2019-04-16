using System.Threading.Tasks;
using Angelo.Common.Migrations;
using Angelo.Connect.Data;
using Microsoft.EntityFrameworkCore;

namespace Angelo.Connect.Web.Migrations.Application
{
    public class A16035_AddIsSystemPageColumn : IAppMigration
    {
        public string Id { get; } = "A16035";

        public string Migration { get; } = "Add IsSystemPage Column";

        private ConnectDbContext _connectDb;

        public A16035_AddIsSystemPageColumn(ConnectDbContext connectDb)
        {
            _connectDb = connectDb;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            // Fail if cannot connect to db
            if (_connectDb.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            await _connectDb.Database.ExecuteNonQueryAsync("ALTER TABLE [cms].[PageMaster] ADD [IsSystemPage] bit NOT NULL DEFAULT 0");

            return MigrationResult.Success("Added [IsSystemPage] to PageMaster");
        }
    }
}
