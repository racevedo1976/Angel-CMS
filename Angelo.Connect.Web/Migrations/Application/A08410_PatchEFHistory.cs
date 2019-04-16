using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Migrations;
using Angelo.Common.Extensions;
using Angelo.Connect.Data;
using Angelo.Connect.Logging;
using Angelo.Identity;

namespace Angelo.Connect.Web.Migrations.Application
{
    public class A08410_PatchEFHistory : IAppMigration
    {
        public string Id { get; } = "A08410";

        public string Migration { get; } = "Patch EF History Table";
     
        private ConnectDbContext _connectDb;
        private IdentityDbContext _identityDb;

        public A08410_PatchEFHistory(ConnectDbContext connectDb, IdentityDbContext identityDb)
        {
            _connectDb = connectDb;
            _identityDb = identityDb;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            var output = new System.Text.StringBuilder();

            var newSchema = "app";
            var newTableName = "MigrationsEF";
            var oldSchema = "dbo";
            var oldTableName = "__EFMigrationsHistory";

            // If the database does not yet exist, we can skip because EF
            // will build it's internal migration tables correctly using new names

            if(_connectDb.Database.TryTestConnection() == true)
            {
                // otherwise, manually insert schema (if needed)
                if (_connectDb.Database.SchemaExists(newSchema) == false)
                {
                    _connectDb.Database.CreateSchema(newSchema);
                    output.Append("Created Connect App Schema. ");
                }

                // then alter old EF migration table (if old table exists)
                if (_connectDb.Database.TableExists(oldTableName, oldSchema) == true)
                {
                    _connectDb.Database.AlterTableName(oldTableName, oldSchema, newTableName);
                    _connectDb.Database.AlterTableSchema(newTableName, oldSchema, newSchema);

                    output.Append("Altered Connect EF Table. ");
                }
            }



            // Rinse & repeat for identity db
            if (_identityDb.Database.TryTestConnection() == true)
            {
                // manually insert schema (if needed)
                if (_identityDb.Database.SchemaExists(newSchema) == false)
                {
                    _identityDb.Database.CreateSchema(newSchema);
                    output.Append("Created Identity App Schema. ");
                }

                // alter EF migration table (if old schema.table exists)
                if (_identityDb.Database.TableExists(oldTableName, oldSchema))
                {
                    _identityDb.Database.AlterTableName(oldTableName, oldSchema, newTableName);
                    _identityDb.Database.AlterTableSchema(newTableName, oldSchema, newSchema);

                    output.Append("Altered Identity EF Table. ");
                }
            }


            // check our log to determine if we did any work and return result
            var message = output.ToString().Trim();

            var result = string.IsNullOrEmpty(message)
                ? MigrationResult.Skipped()
                : MigrationResult.Success(message);

            return await Task.FromResult(result);
        }
    }
}
