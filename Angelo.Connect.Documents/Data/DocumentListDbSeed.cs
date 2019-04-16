using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Angelo.Connect.Documents.Data
{
    public static class DocumentListDbSeed
    {
        public static void CreateSchemas(DocumentListDbContext dbContext)
        {
            dbContext.Database.ExecuteSqlCommand(@"
              IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'plugin')
                BEGIN
                    EXEC('CREATE SCHEMA plugin')
                END
            ");
        }

        public static void CreateTables(DocumentListDbContext dbContext)
        {
            if (!dbContext.Database.TableExists("plugin.DocumentListWidget"))
            {
                dbContext.Database.ExecuteSqlCommand(@"
                    CREATE TABLE [plugin].[DocumentListWidget](
	                    [Id] [nvarchar](50) NOT NULL,
                        [SiteId] [nvarchar](50) NOT NULL,
                        [Title] [nvarchar](500) NULL,
                        CONSTRAINT [PK_DocumentListWidget] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )
                ");
            }

            if (!dbContext.Database.TableExists("plugin.DocumentListDocument"))
            {
                dbContext.Database.ExecuteSqlCommand(@"
                    CREATE TABLE [plugin].[DocumentListDocument](
                        [Id] [nvarchar](50) NOT NULL,
                        [WidgetId] [nvarchar](50) NOT NULL,
                        [DocumentId] [nvarchar](50) NULL,
                        [Title] [nvarchar](500) NULL,
                        [Url] [nvarchar](2048) NULL,
                        [ThumbnailUrl] [nvarchar](2048)  NULL,
                        [FolderId] [nvarchar](50) NULL,
                        [Sort] [int] NULL,
                        CONSTRAINT [PK_DocumentListDocument] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )
                ");
            }

            if (!dbContext.Database.TableExists("plugin.DocumentListFolder"))
            {
                dbContext.Database.ExecuteSqlCommand(@"
                    CREATE TABLE [plugin].[DocumentListFolder](
                        [Id] [nvarchar](50) NOT NULL,
                        [WidgetId] [nvarchar](50) NULL,
                        [Title] [nvarchar](500) NULL,
                        [Sort] [int] NULL,
                        CONSTRAINT [PK_DocumentListFolder] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )
                ");
            }
        }

        public static void InsertSeedData(DocumentListDbContext dbContext)
        {
        }
    }
}
