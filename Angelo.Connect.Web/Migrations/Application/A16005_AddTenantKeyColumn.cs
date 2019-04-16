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
    public class A16005_AddTenantKeyColumn: IAppMigration
    {
        public string Id { get; } = "A16005";

        public string Migration { get; } = "Add Tenant Key Column";
     
        private IdentityDbContext _identityDb;

        public A16005_AddTenantKeyColumn(IdentityDbContext identityDb)
        {
            _identityDb = identityDb;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            var output = new System.Text.StringBuilder();

            // Fail if cannot connect to db
            if(_identityDb.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            // Skip if Tenant.Key already exists
            if (_identityDb.Database.HasColumn("Tenant", "auth", "Key"))
                return MigrationResult.Skipped("Column Tenant.Key already exists.");

            await _identityDb.Database.ExecuteNonQueryAsync("ALTER TABLE [auth].[Tenant] ADD [Key] nvarchar(450) NULL");

            return MigrationResult.Success("Added column [Tenant].[Key]");         
        }
    }
}
