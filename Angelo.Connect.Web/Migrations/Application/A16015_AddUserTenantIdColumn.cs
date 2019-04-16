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
    public class A16015_AddUserTenantIdColumn: IAppMigration
    {
        public string Id { get; } = "A16015";

        public string Migration { get; } = "Add TenantId Column to Identity User";
     
        private IdentityDbContext _identityDb;

        public A16015_AddUserTenantIdColumn(IdentityDbContext identityDb)
        {
            _identityDb = identityDb;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            // Fail if cannot connect to db
            if(_identityDb.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");


            // Skip if User.TenantId already exists
            if (_identityDb.Database.HasColumn("User", "auth", "TenantId"))
                return MigrationResult.Skipped("Column [User].[TenantId] already exists.");
            

            // Create the column - allowing nulls temporarily 
            // This will be changed in the 2nd part of this job (A26015)
            await _identityDb.Database.ExecuteNonQueryAsync("ALTER TABLE [auth].[User] ADD [TenantId] nvarchar(450) NULL");

            
            return MigrationResult.Success("Added column [auth].[User].[TenantId].");         
        }
    }
}
