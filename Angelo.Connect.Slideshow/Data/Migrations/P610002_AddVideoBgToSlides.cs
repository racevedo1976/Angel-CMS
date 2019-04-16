using Angelo.Common.Migrations;
using Angelo.Connect.SlideShow.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Slideshow.Data.Migrations
{
    public class P610002_AddVideoBgToSlides : IAppMigration
    {
        private SlideShowDbContext _dbContext;

        public string Id { get; } = "P610002";

        public string Migration { get; } = "Add Video Background Support To Slides";

        public P610002_AddVideoBgToSlides(SlideShowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            // Fail if cannot connect to db
            if (_dbContext.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            // Skip if bg color already exists
            if (_dbContext.Database.HasColumn("SlideShowSlide", "plugin", "UseVideoBackground"))
                return MigrationResult.Skipped("Column SlideShowSlide.UseVideoBackground already exists.");

            // Skip if VideoUrl already exists
            if (_dbContext.Database.HasColumn("SlideShowSlide", "plugin", "VideoUrl"))
                return MigrationResult.Skipped("Column SlideShowSlide.VideoUrl already exists.");

            // Skip if VideoSource already exists
            if (_dbContext.Database.HasColumn("SlideShowSlide", "plugin", "VideoSource"))
                return MigrationResult.Skipped("Column SlideShowSlide.VideoSource already exists.");

            // Skip if EnableVideoSound already exists
            if (_dbContext.Database.HasColumn("SlideShowSlide", "plugin", "EnableVideoSound"))
                return MigrationResult.Skipped("Column SlideShowSlide.EnableVideoSound already exists.");

            await _dbContext.Database.ExecuteNonQueryAsync(
                "ALTER TABLE [plugin].[SlideShowSlide] ADD [UseVideoBackground] [Bit] NULL Default(0)");
            await _dbContext.Database.ExecuteNonQueryAsync(
                "ALTER TABLE [plugin].[SlideShowSlide] ADD [VideoUrl][nvarchar](500) NULL");
            await _dbContext.Database.ExecuteNonQueryAsync(
                "ALTER TABLE [plugin].[SlideShowSlide] ADD [VideoSource][nvarchar](20) NULL");
            await _dbContext.Database.ExecuteNonQueryAsync(
                "ALTER TABLE [plugin].[SlideShowSlide] ADD [EnableVideoSound][Bit] NULL Default(0)");

            // update the table, set all values to 0
            await _dbContext.Database.ExecuteNonQueryAsync("UPDATE [plugin].[SlideShowSlide] SET UseVideoBackground = 0");
            // update the table, set all values to 0
            await _dbContext.Database.ExecuteNonQueryAsync("UPDATE [plugin].[SlideShowSlide] SET EnableVideoSound = 0");

            return MigrationResult.Success(
                "Added Video Background supporting columns");
        }
    }
}