using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Extensions;
using Angelo.Common.Migrations;
using Angelo.Connect.Data;

namespace Angelo.Connect.Web.Migrations.Application
{
    public class A10400_MigrateConnectModels : IAppMigration
    {
        public string Id { get; } = "A10400";

        public string Migration { get; } = "Migrate Connect Models (EF)";
       
        private ConnectDbContext _connectDb;

        public A10400_MigrateConnectModels(ConnectDbContext connectDb)
        {
            _connectDb = connectDb;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            if (_connectDb.RequiresMigration())
            {
                await _connectDb.Database.MigrateAsync();

                // EF migrations sometimes report back complete before the underlying DB transaction 
                // has completed so stalling a bit. This only affects drop / recreate scenarios.
                await Task.Delay(1200);

                return MigrationResult.Success("Created entity models.");
            }

            return MigrationResult.Skipped("No action required");
        }
    }
}
