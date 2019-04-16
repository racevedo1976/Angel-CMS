using Angelo.Common.Migrations;
using Angelo.Connect.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.Migrations.Application
{
    public class A13010_CreateSiteAlertTable : IAppMigration
    {
        public string Id { get; } = "A13010";

        public string Migration { get; } = "Create SiteAlert Table";

        private ConnectDbContext _connectDb;

        public A13010_CreateSiteAlertTable(ConnectDbContext connectDb)
        {
            _connectDb = connectDb;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {

            string schemaName = "cms";
            string tableName = "SiteAlert";
            var output = new System.Text.StringBuilder();

            // Fail if cannot connect to db
            if (_connectDb.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            try
            {

                // CREATE TABLE   
                if (_connectDb.Database.TableExists(tableName, schemaName) == false)
                {
                    await _connectDb.Database.ExecuteSqlCommandAsync($@"
                        CREATE TABLE [{schemaName}].[{tableName}](
                            [Id] nvarchar(50) NOT NULL,
                            [VersionCode] nvarchar(50)  NOT NULL,
                            [SiteId] nvarchar(50) NOT NULL, 
                            [UserId] nvarchar(50) NOT NULL,
                            [ContentTreeId] nvarchar(50) NOT NULL,
                            [Title] nvarchar(450) NULL,
                            [StartDate] datetime  NOT NULL,
                            [EndDate] datetime  NOT NULL,
                            [Status] int  NOT NULL,
                            [Posted] datetime  NOT NULL,
                            CONSTRAINT [PK_SiteAlert_Id] PRIMARY KEY (Id));
                    ");

                }
                else
                {
                    return await Task.FromResult(MigrationResult.Failed($"Table already created: [{schemaName}].[{tableName}]. "));
                }
            }
            catch (Exception e)
            {
                return await Task.FromResult(MigrationResult.Failed($"Error creating table. {e.Message}"));
            }

            return await Task.FromResult(MigrationResult.Success("Created table [cms].[SiteAlert]"));
            
        }


    }
}
