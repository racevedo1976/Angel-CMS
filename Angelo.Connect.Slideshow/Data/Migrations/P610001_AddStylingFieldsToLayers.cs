using Angelo.Common.Migrations;
using Angelo.Connect.SlideShow.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Slideshow.Data.Migrations
{
    public class P610001_AddStylingFieldsToLayers : IAppMigration
    {
        private SlideShowDbContext _dbContext;

        public string Id { get; } = "P610001";

        public string Migration { get; } = "Add Styling Fields to Layers";

        public P610001_AddStylingFieldsToLayers(SlideShowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            // Fail if cannot connect to db
            if (_dbContext.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            // Skip if bg color already exists
            if (_dbContext.Database.HasColumn("SlideShowLayer", "plugin", "BgColor"))
                return MigrationResult.Skipped("Column SlideShowLayer.BgColor already exists.");

            await _dbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[SlideShowLayer] ADD [BgColor] [nvarchar](50) NULL");
            await _dbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[SlideShowLayer] ADD [FontWeight] [nvarchar](50) NULL");
            await _dbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[SlideShowLayer] ADD [FontStyle] [nvarchar](50) NULL");
            await _dbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[SlideShowLayer] ADD [TextDecoration] [nvarchar](50) NULL");

            return MigrationResult.Success("Added columns [SlideShowLayer].[BgColor], [FontWeight], [FontStyle], [TextDecoration] ");
        }
    }
}
