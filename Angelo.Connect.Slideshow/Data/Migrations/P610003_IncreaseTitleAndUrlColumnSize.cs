using Angelo.Common.Migrations;
using Angelo.Connect.SlideShow.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Slideshow.Data.Migrations
{
    public class P610003_IncreaseTitleAndUrlColumnSize : IAppMigration
    {
        private SlideShowDbContext _dbContext;

        public string Id { get; } = "P610003";

        public string Migration { get; } = "Increase Title And Url Column Size ";

        public P610003_IncreaseTitleAndUrlColumnSize(SlideShowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            // Fail if cannot connect to db
            if (_dbContext.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

           
            await _dbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[SlideShowLayer] ALTER COLUMN [Title] [nvarchar](MAX) NULL");
            await _dbContext.Database.ExecuteNonQueryAsync("ALTER TABLE [plugin].[SlideShowLayer] ALTER COLUMN [SourceUrl] [nvarchar](MAX) NULL");

            return MigrationResult.Success("Successful Increase Title And Url Column Size ");
        }
    }
}
