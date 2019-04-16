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
using Angelo.Connect.Blog.Models;
using Angelo.Connect.Blog.Services;

namespace Angelo.Connect.Blog.Data
{
    public class P410025_InsertMissingVersionInfo : IAppMigration
    {
        private BlogDbContext _blogDbContext;
        private ConnectDbContext _connectDbContext;
        private ContentManager _contentManager;

        public string Id { get; } = "P410025";

        public string Migration { get; } = "Insert missing version data for existing blog posts";

        public P410025_InsertMissingVersionInfo(BlogDbContext blogDbContext, ConnectDbContext connectDbContext, ContentManager contentManager)
        {
            _blogDbContext = blogDbContext;
            _connectDbContext = connectDbContext;
            _contentManager = contentManager;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            var blogContentType = BlogManager.CONTENT_TYPE_BLOGPOST;

            // Fail if cannot connect to db
            if (_blogDbContext.Database.TryTestConnection() == false || _connectDbContext.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            // remove any versioning records that are in an invalid state
            await _connectDbContext.Database.ExecuteNonQueryAsync($@"
                DELETE FROM cms.ContentVersion WHERE ContentType = '{blogContentType}' AND JsonData IS NULL
            ");

            // Ensure UserId is set for remaining versions 
            // NOTE: Will only affect QA / Dev environments where UserIds weren't being saved during early test versions
            await _blogDbContext.Database.ExecuteNonQueryAsync(@"
                UPDATE cms.ContentVersion SET UserId = bp.UserId
                FROM plugin.BlogPost bp
                WHERE cms.ContentVersion.ContentId = bp.Id
                    AND cms.ContentVersion.UserId IS NULL
            ");

            // Identify blogs with valid version data to skip in next step
            // NOTE: These won't exist on production but might exist in dev / test environments
            var blogIdsToSkip = await _connectDbContext.ContentVersions
                .Where(x =>
                    x.ContentType == blogContentType
                    && x.JsonData != null
                )
                .Select(x => x.ContentId).ToArrayAsync();


            // Identify blog posts that have missing version data
            var blogPostsToMigrate = await _blogDbContext.BlogPosts
                .Where(x => !blogIdsToSkip.Contains(x.Id))
                .ToListAsync();


            // Insert the missing verision data
            foreach(var blogPost in blogPostsToMigrate)
            {
                var version = new ContentVersion(blogContentType, blogPost.Id);

                version.VersionCode = blogPost.VersionCode;
                version.Created = blogPost.Posted;
                version.Status = blogPost.Published ? ContentStatus.Published : ContentStatus.Draft;
                version.UserId = blogPost.UserId;
                version.JsonData = SerializeVersionData(blogPost);

                _connectDbContext.ContentVersions.Add(version);
            }

            await _connectDbContext.SaveChangesAsync();
           

            return MigrationResult.Success($"Inserted {blogPostsToMigrate.Count} missing BlogPost version records");
        }

        private string SerializeVersionData(BlogPost blogPost)
        {
            var data =  new BlogPost
            {
                Id = blogPost.Id,
                VersionCode = blogPost.VersionCode,
                Status = blogPost.Status,
                Title = blogPost.Title,
                Excerp = blogPost.Excerp,
                Image = blogPost.Image,
                Caption = blogPost.Caption,
                Posted = blogPost.Posted,
                Content = blogPost.Content,
                ContentTreeId = blogPost.ContentTreeId,
                UserId = blogPost.UserId
            };

            return JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Include
            });
        }
    }
}
