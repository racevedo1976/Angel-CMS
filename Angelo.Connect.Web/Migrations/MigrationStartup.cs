using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

using Angelo.Common.Migrations;
using Angelo.Connect.Web.Migrations.Application;

namespace Angelo.Connect.Web
{
    public static class MigrationStartup
    {
        public static IServiceCollection AddCoreMigrations(this IServiceCollection services)
        {
            // patches
            services.AddAppMigration<A08410_PatchEFHistory>();
            services.AddAppMigration<A08440_PatchJobsSchema>();

            // db creation
            services.AddAppMigration<A10040_EnsureConnectDb>();
            services.AddAppMigration<A10020_EnsureIdentityDb>();

            // model creation
            services.AddAppMigration<A10180_MigrateLogModels>();
            services.AddAppMigration<A10200_MigrateIdentityModels>();
            services.AddAppMigration<A10400_MigrateConnectModels>();
            services.AddAppMigration<A12030_CreatePluginSchema>();
            services.AddAppMigration<A12060_CreateWebCacheTable>();
            services.AddAppMigration<A13010_CreateSiteAlertTable>();

            services.AddAppMigration<A16005_AddTenantKeyColumn>();
            services.AddAppMigration<A16010_AddNavScopeColumn>();
            services.AddAppMigration<A16015_AddUserTenantIdColumn>();
            services.AddAppMigration<A16020_AddPageMetaColumns>();
            services.AddAppMigration<A16025_AddVersionDataColumn>();
            services.AddAppMigration<A16030_UpdateNavMenuTargetType>();
            services.AddAppMigration<A16035_AddIsSystemPageColumn>();

            // data manipulation
            services.AddAppMigration<A24200_SeedEverything>();
            services.AddAppMigration<A24205_SeedInternalProduct>();
            services.AddAppMigration<A24206_SeedCorpUserClaim>();
            services.AddAppMigration<A24207_AddIsHomePageColumn>();
            services.AddAppMigration<A26005_DecoupleTenantKey>();
            services.AddAppMigration<A26015_UpdateUserTenantId>();
            services.AddAppMigration<A26020_UpdateUserLibraryClaims>();
            services.AddAppMigration<A26021_UpdateBoostMobileName>();
            services.AddAppMigration<A26022_InsertMasterPageForMCPSS>();




            return services;
        }
    }
}
