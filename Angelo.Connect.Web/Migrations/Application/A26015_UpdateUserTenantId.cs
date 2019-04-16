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
    public class A26015_UpdateUserTenantId: IAppMigration
    {
        // NOTE: This is the data update portion of A10201

        public string Id { get; } = "A26015";

        public string Migration { get; } = "Update User Tenant Ids";
     
        private IdentityDbContext _identityDb;

        public A26015_UpdateUserTenantId(IdentityDbContext identityDb)
        {
            _identityDb = identityDb;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            var output = new System.Text.StringBuilder();

            // Fail if cannot connect to db
            if (_identityDb.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            // We'll be introducing a new FK constraint. 
            // If it already exist, that means we should skip this process
            if(_identityDb.Database.ConstraintExists("User", "auth", "FK_User_Tenant_TenantId"))
                return MigrationResult.Skipped();

            // [1] Update existing records with TenantId
            await _identityDb.Database.ExecuteNonQueryAsync(@"
                UPDATE [auth].[User]
                SET 
                    TenantId = dir.TenantId
                FROM 
                    [auth].[Directory] dir
                WHERE
                    DirectoryId = dir.Id
            ");


            // [2] Update column to not allow nulls
            await _identityDb.Database.ExecuteNonQueryAsync(@"
                ALTER TABLE [auth].[User] ALTER COLUMN [TenantId] nvarchar(450) NOT NULL
            ");

            // [3] Add FK Constraint on Tenant table
            await _identityDb.Database.ExecuteNonQueryAsync(@"
                ALTER TABLE [auth].[User]
                ADD CONSTRAINT [FK_User_Tenant_TenantId] 
                    FOREIGN KEY ([TenantId]) 
                    REFERENCES [auth].[Tenant] ([Id])
            ");

            // [4] Change UserName from varchar(max) to varchar(450) max can't be used in constraints
            await _identityDb.Database.ExecuteNonQueryAsync(@"
                ALTER TABLE [auth].[User] ALTER COLUMN [UserName] nvarchar(450) NOT NULL
            ");


            // [5] Add a unique constraint (alternate key) on TenantId & UserName
            await _identityDb.Database.ExecuteNonQueryAsync(@"
                ALTER TABLE [auth].[User]
                    ADD CONSTRAINT [AK_TenantId_UserName] UNIQUE (TenantId, UserName); 
            ");


            // [6] Create an index on TenantId
            await _identityDb.Database.ExecuteNonQueryAsync(@"
                CREATE NONCLUSTERED INDEX [IX_User_TenantId]
                    ON [auth].[User]([TenantId] ASC);
            ");

            

            return MigrationResult.Success("Updated table data and created constraints.");
        }
    }
}
