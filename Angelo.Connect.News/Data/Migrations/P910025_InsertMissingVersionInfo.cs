using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using Angelo.Common.Migrations;
using Angelo.Connect.Data;
using Angelo.Connect.Models;
using Angelo.Connect.News.Models;
using Angelo.Connect.News.Services;

namespace Angelo.Connect.News.Data
{
    public class P910025_InsertMissingVersionInfo : IAppMigration
    {
        private NewsDbContext _NewsDbContext;
        private ConnectDbContext _connectDbContext;
        

        public string Id { get; } = "P910025";

        public string Migration { get; } = "Insert missing version data for existing News posts";

        public P910025_InsertMissingVersionInfo(NewsDbContext newsDbContext, ConnectDbContext connectDbContext)
        {
            _NewsDbContext = newsDbContext;
            _connectDbContext = connectDbContext;
           
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            var NewsContentType = NewsManager.CONTENT_TYPE_NEWSPOST;

            // Fail if cannot connect to db
            if (_NewsDbContext.Database.TryTestConnection() == false || _connectDbContext.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            // remove any versioning records that are in an invalid state
            await _connectDbContext.Database.ExecuteNonQueryAsync($@"
                DELETE FROM cms.ContentVersion WHERE ContentType = '{NewsContentType}' AND JsonData IS NULL
            ");

            // Ensure UserId is set for remaining versions 
            // NOTE: Will only affect QA / Dev environments where UserIds weren't being saved during early test versions
            await _NewsDbContext.Database.ExecuteNonQueryAsync(@"
                UPDATE cms.ContentVersion SET UserId = bp.UserId
                FROM plugin.NewsPost bp
                WHERE cms.ContentVersion.ContentId = bp.Id
                    AND cms.ContentVersion.UserId IS NULL
            ");

            // Identify News with valid version data to skip in next step
            // NOTE: These won't exist on production but might exist in dev / test environments
            var newsIdsToSkip = await _connectDbContext.ContentVersions
                .Where(x =>
                    x.ContentType == NewsContentType
                    && x.JsonData != null
                )
                .Select(x => x.ContentId).ToArrayAsync();


            // Identify News posts that have missing version data
            var newsPostsToMigrate = await _NewsDbContext.NewsPosts
                .Where(x => !newsIdsToSkip.Contains(x.Id))
                .ToListAsync();


            // Insert the missing verision data
            foreach(var newsPost in newsPostsToMigrate)
            {
                var version = new ContentVersion(NewsContentType, newsPost.Id);

                version.VersionCode = newsPost.VersionCode;
                version.Created = newsPost.Posted;
                version.Status = newsPost.Published ? ContentStatus.Published : ContentStatus.Draft;
                version.UserId = newsPost.UserId;
                version.JsonData = SerializeVersionData(newsPost);

                _connectDbContext.ContentVersions.Add(version);
            }

            await _connectDbContext.SaveChangesAsync();
           

            return MigrationResult.Success($"Inserted {newsPostsToMigrate.Count} missing NewsPost version records");
        }

        private string SerializeVersionData(NewsPost newsPost)
        {
            var data =  new NewsPost
            {
                Id = newsPost.Id,
                VersionCode = newsPost.VersionCode,
                Status = newsPost.Status,
                Title = newsPost.Title,
                Excerp = newsPost.Excerp,
                Image = newsPost.Image,
                Caption = newsPost.Caption,
                Posted = newsPost.Posted,
                Content = newsPost.Content,
                ContentTreeId = newsPost.ContentTreeId,
                UserId = newsPost.UserId
            };

            return JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Include
            });
        }
    }
}
