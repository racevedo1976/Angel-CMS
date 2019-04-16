using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Common.Migrations;
using Microsoft.EntityFrameworkCore;

namespace Angelo.Connect.Documents.Data.Migrations
{
    public class P510000_CreateInitialDocumentTables : IAppMigration
    {
        public string Id { get; } = "P510000";

        public string Migration { get; } = "Create Initial Documents Tables";

        private DocumentListDbContext _dbContext;

        public P510000_CreateInitialDocumentTables(DocumentListDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            if (_dbContext.Database.TableExists("DocumentListWidget", "plugin"))
                return MigrationResult.Skipped();


            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                    CREATE TABLE [plugin].[DocumentListWidget](
	                    [Id] [nvarchar](50) NOT NULL,
                        [SiteId] [nvarchar](50) NOT NULL,
                        [Title] [nvarchar](500) NULL,
                        CONSTRAINT [PK_DocumentListWidget] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )
                ");

            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                    CREATE TABLE [plugin].[DocumentListDocument](
                        [Id] [nvarchar](50) NOT NULL,
                        [WidgetId] [nvarchar](50) NOT NULL,
                        [DocumentId] [nvarchar](50) NULL,
                        [Title] [nvarchar](500) NULL,
                        [Url] [nvarchar](2048) NULL,
                        [ThumbnailUrl] [nvarchar](2048)  NULL
                        CONSTRAINT [PK_DocumentListDocument] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )
                ");


            return MigrationResult.Success();
        }
    }
}
