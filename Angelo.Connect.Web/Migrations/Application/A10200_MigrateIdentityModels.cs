using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Extensions;
using Angelo.Common.Migrations;
using Angelo.Identity;

namespace Angelo.Connect.Web.Migrations.Application
{
    public class A10200_MigrateIdentityModels : IAppMigration
    {
        public string Id { get; } = "A10200";

        public string Migration { get; } = "Migrate Identity Models (EF)";
     

        private IdentityDbContext _identityDb;

        public A10200_MigrateIdentityModels(IdentityDbContext identityDb)
        {
            _identityDb = identityDb;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            if (_identityDb.RequiresMigration())
            {
                await _identityDb.Database.MigrateAsync();

                // EF migrations sometimes report back complete before the underlying DB transaction 
                // has completed so stalling a bit. This only affects drop / recreate scenarios.
                await Task.Delay(1200);

                return MigrationResult.Success("Created entity models");
            }

            return MigrationResult.Skipped("No action required");
        }
    }
}
