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
    public class A16020_AddPageMetaColumns: IAppMigration
    {
        public string Id { get; } = "A16020";

        public string Migration { get; } = "Add Search Meta Columns to SitePage";
     
        private ConnectDbContext _connectDb;

        public A16020_AddPageMetaColumns(ConnectDbContext connectDb)
        {
            _connectDb = connectDb;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            // Fail if cannot connect to db
            if(_connectDb.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            if (_connectDb.Database.HasColumn("Page", "cms", "Keywords"))
                return MigrationResult.Skipped("Column [Page].[Keywords] already exists.");
            
            // Keywords column
            await _connectDb.Database.ExecuteNonQueryAsync(
                "ALTER TABLE [cms].[Page] ADD [Keywords] nvarchar(1000) NULL"
            );

            // Summary column
            await _connectDb.Database.ExecuteNonQueryAsync(
               "ALTER TABLE [cms].[Page] ADD [Summary] nvarchar(2000) NULL"
           );


            return MigrationResult.Success("Added Keywords & Summary columns.");         
        }
    }
}
