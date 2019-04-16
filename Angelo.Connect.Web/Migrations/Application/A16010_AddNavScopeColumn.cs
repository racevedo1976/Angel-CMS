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
    public class A16010_AddNavScopeColumn: IAppMigration
    {
        public string Id { get; } = "A16010";

        public string Migration { get; } = "Add Navigation Scope Column";
     
        private ConnectDbContext _connectDb;

        public A16010_AddNavScopeColumn(ConnectDbContext connectDb)
        {
            _connectDb = connectDb;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            // Fail if cannot connect to db
            if(_connectDb.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");


            // Skip if Tenant.Key already exists
            if (_connectDb.Database.HasColumn("NavigationMenu", "cms", "Scope"))
                return MigrationResult.Skipped("Column [NavigationMenu].[Scope] already exists.");
            
            // [1]
            // Add the new column
            await _connectDb.Database.ExecuteNonQueryAsync("ALTER TABLE [cms].[NavigationMenu] ADD [Scope] nvarchar(450) NULL");
            

            // [2]
            // Fix NavMenus that previously had their ID "hacked" as a way to identify the main menu
            // Use the new Scope column instead to identify the main menu

            // [2.1] DROP FK and PK ID constraints since we'll be operating on Ids          
            await _connectDb.Database.ExecuteNonQueryAsync(@"
                ALTER TABLE [cms].[NavigationMenuItem] 
                    DROP CONSTRAINT [FK_NavigationMenuItem_NavigationMenu_NavMenuId]
            ");

            await _connectDb.Database.ExecuteNonQueryAsync(@"
                ALTER TABLE [cms].[NavigationMenu] 
                    DROP CONSTRAINT [PK_NavigationMenu]
            ");



            // [2.2]
            // UPDATE the Hacky 'main-' Ids. Use scope = main instead
            await _connectDb.Database.ExecuteNonQueryAsync(@"
                UPDATE [cms].[NavigationMenu]
                SET 
                    [Id] = REPLACE([Id], 'main-', ''),
                    [Scope] = 'main'
                WHERE
                    [Id] LIKE 'main-%'
            ");

            await _connectDb.Database.ExecuteNonQueryAsync(@"
                UPDATE [cms].[NavigationMenuItem]
                SET 
                    [NavMenuId] = REPLACE([NavMenuId], 'main-', '')
                WHERE
                    [NavMenuId] LIKE 'main-%'
            ");


            // [2.3]
            // Add the PK and FK constraints back
            await _connectDb.Database.ExecuteNonQueryAsync(@"
                ALTER TABLE [cms].[NavigationMenu]
                    ADD CONSTRAINT [PK_NavigationMenu] PRIMARY KEY CLUSTERED ([Id] ASC)
            ");

            await _connectDb.Database.ExecuteNonQueryAsync(@"
                ALTER TABLE [cms].[NavigationMenuItem]
                ADD CONSTRAINT [FK_NavigationMenuItem_NavigationMenu_NavMenuId] 
                    FOREIGN KEY ([NavMenuId]) 
                    REFERENCES [cms].[NavigationMenu] ([Id])
            ");

            // [2.4]
            // Fix NavMenuId's in the Navigation Widget Table
            // Note: The widget table does not have a FK constraint on Navigation Menu
            //       so okay to do this last
            if(_connectDb.Database.TableExists("NavMenuWidget", "plugin"))
            {
                await _connectDb.Database.ExecuteNonQueryAsync(@"
                    UPDATE 
                        [plugin].[NavMenuWidget]
                    SET
                        [NavMenuId] = REPLACE([NavMenuId], 'main-', '')
                    WHERE
                        [NavMenuId] LIKE 'main-%'
                ");
            }

            return MigrationResult.Success("Added column [NavigationMenu].[Scope]. Updated scope values for site menus.");         
        }
    }
}
