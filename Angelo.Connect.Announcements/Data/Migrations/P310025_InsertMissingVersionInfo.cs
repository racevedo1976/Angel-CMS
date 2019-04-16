using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using Angelo.Common.Migrations;
using Angelo.Connect.Data;
using Angelo.Connect.Models;
using Angelo.Connect.Services;
using Angelo.Connect.Announcement.Models;
using Angelo.Connect.Announcement.Services;

namespace Angelo.Connect.Announcement.Data
{
    public class P310025_InsertMissingVersionInfo : IAppMigration
    {
        private AnnouncementDbContext _announcementDbContext;
        private ConnectDbContext _connectDbContext;
        private ContentManager _contentManager;

        public string Id { get; } = "P310025";

        public string Migration { get; } = "Insert missing version data for existing announcement posts";

        public P310025_InsertMissingVersionInfo(AnnouncementDbContext announcementDbContext, ConnectDbContext connectDbContext, ContentManager contentManager)
        {
            _announcementDbContext = announcementDbContext;
            _connectDbContext = connectDbContext;
            _contentManager = contentManager;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            var announcementContentType = AnnouncementManager.CONTENT_TYPE_ANNOUNCEMENTPOST;

            // Fail if cannot connect to db
            if (_announcementDbContext.Database.TryTestConnection() == false || _connectDbContext.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            // remove any versioning records that are in an invalid state
            await _connectDbContext.Database.ExecuteNonQueryAsync($@"
                DELETE FROM cms.ContentVersion WHERE ContentType = '{announcementContentType}' AND JsonData IS NULL
            ");

            // Ensure UserId is set for remaining versions 
            // NOTE: Will only affect QA / Dev environments where UserIds weren't being saved during early test versions
            await _announcementDbContext.Database.ExecuteNonQueryAsync(@"
                UPDATE cms.ContentVersion SET UserId = bp.UserId
                FROM plugin.AnnouncementPost bp
                WHERE cms.ContentVersion.ContentId = bp.Id
                    AND cms.ContentVersion.UserId IS NULL
            ");

            // Identify announcements with valid version data to skip in next step
            // NOTE: These won't exist on production but might exist in dev / test environments
            var announcementIdsToSkip = await _connectDbContext.ContentVersions
                .Where(x =>
                    x.ContentType == announcementContentType
                    && x.JsonData != null
                )
                .Select(x => x.ContentId).ToArrayAsync();


            // Identify announcement posts that have missing version data
            var announcementPostsToMigrate = await _announcementDbContext.AnnouncementPosts
                .Where(x => !announcementIdsToSkip.Contains(x.Id))
                .ToListAsync();


            // Insert the missing verision data
            foreach(var announcementPost in announcementPostsToMigrate)
            {
                var version = new ContentVersion(announcementContentType, announcementPost.Id);

                version.VersionCode = announcementPost.VersionCode;
                version.Created = announcementPost.Posted;
                version.Status = announcementPost.Published ? ContentStatus.Published : ContentStatus.Draft;
                version.UserId = announcementPost.UserId;
                version.JsonData = SerializeVersionData(announcementPost);

                _connectDbContext.ContentVersions.Add(version);
            }

            await _connectDbContext.SaveChangesAsync();
           

            return MigrationResult.Success($"Inserted {announcementPostsToMigrate.Count} missing AnnouncementPost version records");
        }

        private string SerializeVersionData(AnnouncementPost announcementPost)
        {
            var data =  new AnnouncementPost
            {
                Id = announcementPost.Id,
                VersionCode = announcementPost.VersionCode,
                Status = announcementPost.Status,
                Title = announcementPost.Title,
                Excerp = announcementPost.Excerp,
                Image = announcementPost.Image,
                Caption = announcementPost.Caption,
                Posted = announcementPost.Posted,
                Content = announcementPost.Content,
                ContentTreeId = announcementPost.ContentTreeId,
                UserId = announcementPost.UserId
            };

            return JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Include
            });
        }
    }
}
