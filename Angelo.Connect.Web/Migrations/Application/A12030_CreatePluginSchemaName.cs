using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Migrations;
using Angelo.Connect.Data;

namespace Angelo.Connect.Web.Migrations.Application
{
    public class A12030_CreatePluginSchema : IAppMigration
    {
        public string Id { get; } = "A12030";

        public string Migration { get; } = "Create Plugins Schema Name";
      

        private ConnectDbContext _connectDb;

        public A12030_CreatePluginSchema(ConnectDbContext connectDb)
        {
            _connectDb = connectDb;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            var schemaName = "plugin";

            // if schema exists, we bail out
            if (_connectDb.Database.SchemaExists(schemaName))
                return MigrationResult.Skipped();

            // otherwise create the schema
            await _connectDb.Database.ExecuteSqlCommandAsync($@"
                    EXEC('CREATE SCHEMA [{schemaName}]')
            ");

            return MigrationResult.Success($"Created schema [{schemaName}]");
        }
    }
}
