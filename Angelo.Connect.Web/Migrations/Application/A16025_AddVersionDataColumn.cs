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
    public class A16025_AddVersionDataColumn: IAppMigration
    {
        public string Id { get; } = "A16025";

        public string Migration { get; } = "Add JsonData column to ContentVersion";
     
        private ConnectDbContext _connectDb;

        public A16025_AddVersionDataColumn(ConnectDbContext connectDb)
        {
            _connectDb = connectDb;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            // Fail if cannot connect to db
            if(_connectDb.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            if (_connectDb.Database.HasColumn("ContentVersion", "cms", "JsonData"))
                return MigrationResult.Skipped("Column [ContentVersion].[JsonData] already exists.");
            

            await _connectDb.Database.ExecuteNonQueryAsync(
                "ALTER TABLE [cms].[ContentVersion] ADD [JsonData] nvarchar(max) NULL"
            );


            return MigrationResult.Success("Added Keywords & Summary columns.");         
        }
    }
}
