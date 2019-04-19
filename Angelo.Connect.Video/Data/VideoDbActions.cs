using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace Angelo.Connect.Video.Data
{
    public static class VideoDbActions
    {

        public static void CreateSchemas(VideoDbContext dbContext)
        {
            dbContext.Database.ExecuteSqlCommand(@"
                IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'plugin')
                BEGIN
                    EXEC('CREATE SCHEMA plugin')
                END
            ");
        }

        public static void CreateTables(VideoDbContext dbContext)
        {
            if (!dbContext.Database.TableExists("plugin.VideoWidget"))
            {
                dbContext.Database.ExecuteSqlCommand(@"
                    CREATE TABLE [plugin].[VideoWidget](
	                    [Id] [nvarchar](50) NOT NULL,
	                    [Title] [nvarchar](500) NULL,
                        [VideoSourceType] [nvarchar](500) NULL,
	                    [VideoId] [nvarchar](500) NULL,
                        [VideoUrl] [nvarchar](500) NULL,
	                    CONSTRAINT [PK_VideoLink] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )
                ");
            }

            if (!dbContext.Database.TableExists("plugin.VideoStreamLink"))
            {
                dbContext.Database.ExecuteSqlCommand(@"
                    CREATE TABLE [plugin].[VideoStreamLink](
	                    [Id] [nvarchar](50) NOT NULL,
                        [ClientId] [nvarchar](50) NULL,
	                    [Title] [nvarchar](500) NULL,
	                    [Path] [nvarchar](500) NULL,
	                    CONSTRAINT [PK_VideoLinkSource] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )
                ");
            }
        }

        public static void InsertSeedData(VideoDbContext dbContext)
        {
            var clientId = "MyCompany";

            if(dbContext.VideoStreamLinks.Count() == 0)
            {
                dbContext.VideoStreamLinks.Add(new Models.VideoStreamLink()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Title = "Video 1 - Whitley Elementary",
                    Path = "http://mcpss.tv/Clients/7e26f301-1c4d-4cb9-b8fa-05df81b54e3d2015%20Show%201%20Whitley%20Elementary-m3u8-aapl.ism/manifest(format=m3u8-aapl).m3u8",
                    ClientId = clientId
                });

                dbContext.VideoStreamLinks.Add(new Models.VideoStreamLink()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Title = "Video 2 - Denisha Mckenzie",
                    Path = "http://mcpss.tv/Clients/028436e4-7fe2-4ead-bb37-e06193500a64Denisha%20Mckenzie%20final-m3u8-aapl.ism/manifest(format=m3u8-aapl).m3u8",
                    ClientId = clientId
                });

                dbContext.VideoStreamLinks.Add(new Models.VideoStreamLink()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Title = "Video 3 - Live Feed",
                    Path = "http://216.66.22.75:1935/live/mpegts.stream/playlist.m3u8",
                    ClientId = clientId
                });

                dbContext.VideoStreamLinks.Add(new Models.VideoStreamLink()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Title = "Cute Kittens",
                    Path = "http://youtube.com/watch?v=w6DW4i-mfbA",
                    ClientId = clientId
                });

                dbContext.VideoStreamLinks.Add(new Models.VideoStreamLink()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Title = "Cute Puppies",
                    Path = "http://youtube.com/watch?v=mRf3-JkwqfU",
                    ClientId = clientId
                });

                dbContext.VideoStreamLinks.Add(new Models.VideoStreamLink()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Title = "Battle Bots",
                    Path = "http://youtube.com/watch?v=2ixB9k6aijw",
                    ClientId = clientId
                });

                dbContext.VideoStreamLinks.Add(new Models.VideoStreamLink()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Title = "Low Flying and Sonic Boom Jets",
                    Path = "http://youtube.com/watch?v=9f2wBmFC3Rk",
                    ClientId = clientId
                });

                dbContext.VideoStreamLinks.Add(new Models.VideoStreamLink()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Title = "The Office Best Moments",
                    Path = "http://youtube.com/watch?v=3OYtjXVilsA",
                    ClientId = clientId
                });

                dbContext.SaveChanges();
            }
        }
    }
}
