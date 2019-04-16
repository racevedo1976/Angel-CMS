using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Extensions;
using Angelo.Common.Migrations;
using Angelo.Connect.Logging;

namespace Angelo.Connect.Web.Migrations.Application
{
    public class A10180_MigrateLogModels : IAppMigration
    {
        public string Id { get; } = "A10180";

        public string Migration { get; } = "Migrate Log Models (EF)";
       

        private DbLogContext _loggerDb;

        public A10180_MigrateLogModels(DbLogContext loggerDb)
        {
            _loggerDb = loggerDb;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            if (_loggerDb.RequiresMigration())
            {
                await _loggerDb.Database.MigrateAsync();
                return MigrationResult.Success("Created entity models.");
            }

            return MigrationResult.Skipped("No action required");
        }
    }
}
