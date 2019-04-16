using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Migrations;
using Angelo.Common.Extensions;
using Angelo.Connect.Data;
using Angelo.Identity;

namespace Angelo.Connect.Web.Migrations.Application
{
    public class A08440_PatchJobsSchema: IAppMigration
    {
        public string Id { get; } = "A08440";

        public string Migration { get; } = "Patch Jobs Schema";
     
        private ConnectDbContext _connectDb;
        private IdentityDbContext _identityDb;

        public A08440_PatchJobsSchema(ConnectDbContext connectDb)
        {
            _connectDb = connectDb;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            var output = new System.Text.StringBuilder();

            var oldSchema = "jobs";
            var newSchema = "app";
            var chronJobTable = "CronJobs";
            var delayedJobTable = "DelayedJobs";
            var jobMetaTable = "Schema";

            // If the database does not yet exist, we can skip because will be 
            // built correctly on new environments

            if (_connectDb.Database.TryTestConnection() == true)
            {
                // insert new schema (if needed)
                if (_connectDb.Database.SchemaExists(newSchema) == false)
                {
                    _connectDb.Database.CreateSchema(newSchema);
                    output.Append($"Created schema [{newSchema}]. ");
                }


                // Move tables to new schema (if needed)  
                if (_connectDb.Database.TableExists(chronJobTable, newSchema) == false)
                {
                    if (_connectDb.Database.TableExists(chronJobTable, oldSchema) == true)
                    {
                        _connectDb.Database.AlterTableSchema(chronJobTable, oldSchema, newSchema);
                        output.Append($"Altered table [{chronJobTable}]. ");
                    }
                }

                if (_connectDb.Database.TableExists(delayedJobTable, newSchema) == false)
                {
                    if (_connectDb.Database.TableExists(delayedJobTable, oldSchema) == true)
                    {
                        _connectDb.Database.AlterTableSchema(delayedJobTable, oldSchema, newSchema);
                        output.Append($"Altered table [{delayedJobTable}]. ");
                    }
                }

                // It's possible that new tables and old tables exist side-by-side
                // If so, let's remove the old ones
                if (_connectDb.Database.TableExists(jobMetaTable, oldSchema) == true)
                {
                    _connectDb.Database.DropTable(jobMetaTable, oldSchema);
                    _connectDb.Database.DropTable(chronJobTable, oldSchema);
                    _connectDb.Database.DropTable(delayedJobTable, oldSchema);

                    output.Append($"Removed old jobs tables. ");
                }

                // finally, let's drop the old "jobs" schema if it exists
                if(_connectDb.Database.SchemaExists(oldSchema) == true)
                {
                    _connectDb.Database.DropSchema(oldSchema);
                    output.Append($"Removed old jobs schema. ");
                }
            }

            // if we have any messages, then we did something.
            // otherwise everything got skipped
            var message = output.ToString().Trim();

            var result = string.IsNullOrEmpty(message)
                ? MigrationResult.Skipped()
                : MigrationResult.Success(message);

            return await Task.FromResult(result);         
        }
    }
}
