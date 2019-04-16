using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Migrations;
using Angelo.Connect.Data;

namespace Angelo.Connect.Web.Migrations.Application
{
    public class A12060_CreateWebCacheTable : IAppMigration
    {
        public string Id { get; } = "A12060";

        public string Migration { get; } = "Create WebCache Table";      

        private ConnectDbContext _connectDb;

        public A12060_CreateWebCacheTable(ConnectDbContext connectDb)
        {
            _connectDb = connectDb;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
           
            string schemaName = "app";
            string tableName = "WebCache";
            var output = new System.Text.StringBuilder();

            // create schema if needed
            if (_connectDb.Database.SchemaExists(schemaName) == false)
            {
                await _connectDb.Database.ExecuteSqlCommandAsync($@"
                    CREATE SCHEMA [{schemaName}]
                ");

                output.Append($"Created schema [{schemaName}]. ");
            }


            // CREATE TABLE SCHEMA (if needed) 
            if (_connectDb.Database.TableExists(tableName, schemaName) == false)
            {
                await _connectDb.Database.ExecuteSqlCommandAsync($@"
                    CREATE TABLE [{schemaName}].[{tableName}](
                        [Id] nvarchar(450) COLLATE SQL_Latin1_General_CP1_CS_AS NOT NULL,
                        [Value] varbinary(MAX) NOT NULL, 
                        [ExpiresAtTime] datetimeoffset NOT NULL,
                        [SlidingExpirationInSeconds] bigint NULL,
                        [AbsoluteExpiration] datetimeoffset NULL,
                        CONSTRAINT [PK_Id] PRIMARY KEY (Id));

                    CREATE NONCLUSTERED INDEX [IX_ExpiresAtTime] ON [{schemaName}].[{tableName}](ExpiresAtTime);
                ");

                output.Append($"Created [{schemaName}].[{tableName}]. ");
            }

            // Cleanup old dbo version if present
            if (_connectDb.Database.TableExists("WebCache", "dbo") == true)
            {
                // drop it
                await _connectDb.Database.ExecuteSqlCommandAsync(@"
                    DROP TABLE [dbo].[WebCache]
                ");

                output.Append("Removed [dbo].[WebCache] (old version). ");
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
