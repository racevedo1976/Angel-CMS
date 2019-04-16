using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Angelo.Connect.SlideShow.Data
{
    public static class GalleryDbSeed
    {
        public static void CreateSchemas(GalleryDbContext dbContext)
        {
            //shouldnt this be taken care of by the plugin framework??
            dbContext.Database.ExecuteSqlCommand(@"
              IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'plugin')
                BEGIN
                    EXEC('CREATE SCHEMA plugin')
                END
            ");
        }

        public static void CreateTables(GalleryDbContext dbContext)
        {
            if (!dbContext.Database.TableExists("plugin.GalleryWidget"))
            {
                dbContext.Database.ExecuteSqlCommand(@"
                    CREATE TABLE [plugin].[GalleryWidget](
	                    [Id] [nvarchar](50) NOT NULL,
                        [SiteId] [nvarchar](50) NOT NULL,
	                    
                        [Title] [nvarchar](500) NULL,
                        [GalleryId] [nvarchar](50) NULL,
                        CONSTRAINT [PK_GalleryWidget] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )
                ");
            }

            if (!dbContext.Database.TableExists("plugin.GalleryItem"))
            {
                dbContext.Database.ExecuteSqlCommand(@"
                    CREATE TABLE [plugin].[GalleryItem](
                        [Id] [nvarchar](50) NOT NULL,
                        [WidgetId] [nvarchar](50) NOT NULL,
                        [Title] [nvarchar](500) NULL,
                        [IsLinkEnabled] [bit]  NULL,
                        [Url] [nvarchar](2048) NULL,
                        [LinkUrl] [nvarchar](2048) NULL,
                        [LinkTarget] [INT]  NULL,
                        [ThumbnailUrl] [nvarchar](2048)  NULL,

                        CONSTRAINT [PK_GalleryItem] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )
                ");
            }
        }

        public static void InsertSeedData(GalleryDbContext dbContext)
        {
        }
    }
}
