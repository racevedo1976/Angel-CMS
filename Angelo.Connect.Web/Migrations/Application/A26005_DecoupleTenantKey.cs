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
    public class A26005_DecoupleTenantKey: IAppMigration
    {
        // NOTE: This is the data update portion of A10201

        public string Id { get; } = "A26005";

        public string Migration { get; } = "Decouple Tenant Key";
     
        private IdentityDbContext _identityDb;

        public A26005_DecoupleTenantKey(IdentityDbContext identityDb)
        {
            _identityDb = identityDb;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            var output = new System.Text.StringBuilder();

            // Fail if cannot connect to db
            if (_identityDb.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            var existingTenants = await _identityDb.Tenants.ToListAsync();

            // Skip if any Tenant.Key data exists (all should be null)
            if (existingTenants.Any(x => x.Key != null))
                return MigrationResult.Skipped("No action required");


            // 1) Remove Index and Foreign Key Constraints for Tenant.Id (temporarily)
            //-------------------------------------------------------------------------------

            // Directories           
            _identityDb.Database.ExecuteNonQuery("ALTER TABLE [auth].[Directory] DROP CONSTRAINT [FK_Directory_Tenant_TenantId]");
            _identityDb.Database.ExecuteNonQuery("DROP INDEX [IX_Directory_TenantId] ON [auth].[Directory]");

            // Security Pools
            _identityDb.Database.ExecuteNonQuery("ALTER TABLE [auth].[SecurityPool] DROP CONSTRAINT [FK_SecurityPool_Tenant_TenantId]");
            _identityDb.Database.ExecuteNonQuery("DROP INDEX [IX_SecurityPool_TenantId] ON [auth].[SecurityPool]");

            // Tenant Uris
            _identityDb.Database.ExecuteNonQuery("ALTER TABLE [auth].[TenantUri] DROP CONSTRAINT [PK_TenantUri]");
            _identityDb.Database.ExecuteNonQuery("ALTER TABLE [auth].[TenantUri] DROP CONSTRAINT [FK_TenantUri_Tenant_TenantId]");

            // Tenant
            _identityDb.Database.ExecuteNonQuery("ALTER TABLE [auth].[Tenant] DROP CONSTRAINT [PK_Tenant]");

            output.Append("Dropping Constraints. ");


            // 2) Update existing data tenant and related data
            //-------------------------------------------------------------------------------
           
            foreach (var tenant in existingTenants)
            {
                var tenantId = KeyGen.NewGuid();
                var tenantKey = tenant.Id;

                _identityDb.Database.ExecuteNonQuery($@"
                    UPDATE [auth].[Tenant] SET [Id] = '{tenantId}', [Key] = '{tenantKey}'
                    WHERE [Id] = '{tenantKey}';

                    UPDATE [auth].[TenantUri] SET [TenantId] = '{tenantId}'
                    WHERE [TenantId] = '{tenantKey}';

                    UPDATE [auth].[SecurityPool] SET [TenantId] = '{tenantId}'
                    WHERE [TenantId] = '{tenantKey}';

                    UPDATE [auth].[Directory] SET [TenantId] = '{tenantId}'
                    WHERE [TenantId] = '{tenantKey}';  
                ");
            }


            output.Append($"Migrated {existingTenants.Count} tenants. ");

            // 3) Add back all previously existing Constraints
            //-------------------------------------------------------------------------------

            // Tenants (also adding new alternate key constraint on Tenant.Key)
            _identityDb.Database.ExecuteNonQuery("ALTER TABLE [auth].[Tenant] ADD CONSTRAINT [PK_Tenant] PRIMARY KEY CLUSTERED ([Id] ASC)");
            _identityDb.Database.ExecuteNonQuery("ALTER TABLE [auth].[Tenant] ADD CONSTRAINT [AK_Tenant_Key] UNIQUE NONCLUSTERED ([Key] ASC)");

            // Tenant Uris
            _identityDb.Database.ExecuteNonQuery("ALTER TABLE [auth].[TenantUri] ADD CONSTRAINT [PK_TenantUri] PRIMARY KEY CLUSTERED ([TenantId] ASC, [Type] ASC, [Uri] ASC)");
            _identityDb.Database.ExecuteNonQuery("ALTER TABLE [auth].[TenantUri] ADD CONSTRAINT [FK_TenantUri_Tenant_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [auth].[Tenant] ([Id])");

            // Directories
            _identityDb.Database.ExecuteNonQuery("ALTER TABLE [auth].[Directory] ADD CONSTRAINT [FK_Directory_Tenant_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [auth].[Tenant] ([Id])");
            _identityDb.Database.ExecuteNonQuery("CREATE NONCLUSTERED INDEX [IX_Directory_TenantId] ON [auth].[Directory]([TenantId] ASC)");

            // Security Pools
            _identityDb.Database.ExecuteNonQuery("ALTER TABLE [auth].[SecurityPool] ADD CONSTRAINT [FK_SecurityPool_Tenant_TenantId] FOREIGN KEY ([TenantId]) REFERENCES [auth].[Tenant] ([Id])");
            _identityDb.Database.ExecuteNonQuery("CREATE NONCLUSTERED INDEX [IX_SecurityPool_TenantId] ON [auth].[SecurityPool]([TenantId] ASC)");

            output.Append("Recreated constraints.");


            // DONE
            return MigrationResult.Success(output.ToString());
        }
    }
}
