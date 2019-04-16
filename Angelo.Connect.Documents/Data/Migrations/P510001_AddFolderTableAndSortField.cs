using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Common.Migrations;
using Microsoft.EntityFrameworkCore;

namespace Angelo.Connect.Documents.Data.Migrations
{
    public class P510001_AddFolderTableAndSortField : IAppMigration
    {
        public string Id { get; } = "P510001";

        public string Migration { get; } = "Add Folder Table And Sort Field";

        private readonly DocumentListDbContext _dbContext;

        public P510001_AddFolderTableAndSortField(DocumentListDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            // Fail if cannot connect to db
            if (_dbContext.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            // Skip if sort already exists
            if (_dbContext.Database.HasColumn("DocumentListDocument", "plugin", "Sort"))
                return MigrationResult.Skipped("Column DocumentListDocument.Sort already exists.");

            await _dbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[DocumentListDocument] ADD [Sort] INT NULL");
            await _dbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[DocumentListDocument] ADD [FolderId] [nvarchar](50) NULL");

            if (_dbContext.Database.TableExists("DocumentListFolder", "plugin"))
                return MigrationResult.Skipped("Table DocumentListFolder is already created.");

            //add the folder table
            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                    CREATE TABLE [plugin].[DocumentListFolder](
                        [Id] [nvarchar](50) NOT NULL,
                        [WidgetId] [nvarchar](50) NULL,
                        [Title] [nvarchar](500) NULL,
                        [Sort] [int] NULL,
                        CONSTRAINT [PK_DocumentListFolder] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )
                ");

            //Update the sort column default value (fix issue in SQL 2008)
            await _dbContext.Database.ExecuteNonQueryAsync("UPDATE [plugin].[DocumentListDocument] SET Sort = 0");

            return MigrationResult.Success("Added column [DocumentListDocument].[Sort] and created DocumentListFolder");
        }
    }
}
