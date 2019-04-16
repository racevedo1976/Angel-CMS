using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Extensions;
using Angelo.Connect.News.Data;
using Angelo.Connect.News.Models;
using Angelo.Connect.News.Security;
using Angelo.Connect.Security;
using Angelo.Connect.Services;
using Angelo.Connect.Models;

namespace Angelo.Connect.News.Services
{
    public class NewsManager
    {
        public const string CONTENT_TYPE_NEWSPOST = "NewsPost";

        private readonly NewsDbContext _newsDbContext;
        private readonly ContentManager _contentManager;
       

        public NewsManager
        (
            ContentManager contentManger,
            NewsDbContext newsDbContext
        )
        {
           _contentManager = contentManger;
           _newsDbContext = newsDbContext;
        }

        public NewsCategory GetNewsCategory(string categoryId)
        {
            return _newsDbContext.NewsCategories.FirstOrDefault(x => x.Id == categoryId);
        }

        // TODO: Replace this version with verison that requires userContext
        //       Since ownership may be claims based 
        public List<NewsCategory> GetNewsCategoriesOwnedByUser(string userId)
        {
            var categories = _newsDbContext.NewsCategories
                .AsNoTracking()
                .Where(x => x.UserId == userId).ToList()
                .OrderBy(x => x.Title)
                .ToList();

            // Ensure navigation properties are null since not tracking 
            categories.ForEach(x => {
                x.NewsPostMap = null;
                x.NewsWidgetMap = null;
            });

            return categories;
        }

        public List<NewsCategory> GetNewsCategoriesSharedWithUser(UserContext userContext)
        {
            // TODO: Filter this list based on User's Claims
            var allowedIds = userContext.SecurityClaims       
                .Where(x => x.Type == NewsClaimTypes.NewsCategoryContribute)
                .Select(x => x.Value)
                .ToList();

            var categories = _newsDbContext.NewsCategories
                .AsNoTracking()
                .Where(x => allowedIds.Contains(x.Id)).ToList()
                .OrderBy(x => x.Title)
                .ToList();

            // Ensure navigation properties are null since not tracking 
            categories.ForEach(x => {
                x.NewsPostMap = null;
                x.NewsWidgetMap = null;
            });

            return categories;
        }

        public List<NewsPost> GetNewsPostsOwnedByUser(string userId)
        {
            return _newsDbContext.NewsPosts
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Posted)
                .ThenBy(x => x.Title)
                .ToList();
        }

        public List<NewsPostTag> GetNewsPostTags(string newsPostId)
        {
            return _newsDbContext.NewsPostTags
                .Where(x => x.PostId == newsPostId)
                .ToList();
        }

        public List<NewsPostCategory> GetNewsPostCategories(string newsPostId)
        {
            return _newsDbContext.NewsPostCategories
                .Include(x => x.Category)
                .Where(x => x.PostId == newsPostId)
                .ToList();
        }

        public NewsPost GetNewsPost(string newsPostId)
        {
            return _newsDbContext
                .NewsPosts
                .Include(x => x.Categories)
                .ThenInclude(y => y.Category)
                .FirstOrDefault(x => x.Id == newsPostId);
        }

        public NewsPost GetNewsPost(string newsPostId, string versionCode)
        {
            var newsPostCurrent = GetNewsPost(newsPostId);

            if (versionCode == null || versionCode == newsPostCurrent.VersionCode)
                return newsPostCurrent;

            // otherwise get the data stored along with the version
            var versionInfo = _contentManager.GetVersionInfo(CONTENT_TYPE_NEWSPOST, newsPostId, versionCode).Result;

            if (versionInfo == null)
                throw new Exception($"Could not get News post. Missing version {versionCode}.");

            var newsPost = newsPostCurrent.Clone();
            var versionData = _contentManager.GetStoredData<NewsPost>(versionInfo);

            MergeVersionData(newsPost, versionData);
            
            return newsPost;
        }

        public ContentVersion GetVersionInfo(string newsPostId, string versionCode = null)
        {
            var newsPost = GetNewsPost(newsPostId);

            if (newsPost == null)
                return null;

            if(versionCode != null)
                return _contentManager.GetVersionInfo(CONTENT_TYPE_NEWSPOST, newsPost.Id, versionCode).Result;
           
            // else
            return newsPost.Published
                ? _contentManager.GetPublishedVersionInfo(CONTENT_TYPE_NEWSPOST, newsPost.Id).Result
                : _contentManager.GetLatestDraftVersionInfo(CONTENT_TYPE_NEWSPOST, newsPost.Id).Result;        
        }

        public IEnumerable<ContentVersion> GetVersionHistory(string newsPostId)
        {
            return _contentManager.GetVersionHistory(CONTENT_TYPE_NEWSPOST, newsPostId).Result;
        }

        public void DeleteNewsPost(NewsPost newsPost)
        {
            // remove content records
            _contentManager.DeleteAllVersions(CONTENT_TYPE_NEWSPOST, newsPost.Id).Wait();
            _contentManager.DeleteAllContentTrees(CONTENT_TYPE_NEWSPOST, newsPost.Id).Wait();

            // remove child associations
            ClearNewsPostCategories(newsPost.Id);
            ClearNewsPostTags(newsPost.Id);

            // remove the post
            _newsDbContext.NewsPosts.Remove(newsPost);
            _newsDbContext.SaveChanges();
        }
      
        public void CreateNewsCategory(NewsCategory newsCategory)
        {
            // ensure no other category exists with this title
            var similarCategory = _newsDbContext.NewsCategories
                .FirstOrDefault(x => 
                    x.Title == newsCategory.Title 
                    && x.UserId == newsCategory.UserId
                );

            if (similarCategory != null)
            {
                throw new Exception($"Cannot create News category. A category named {newsCategory.Title} already exists.");
            }

            // create the new category
            newsCategory.Id = KeyGen.NewGuid();

            _newsDbContext.NewsCategories.Add(newsCategory);
            _newsDbContext.SaveChanges();

        }

        public void UpdateNewsCategory(NewsCategory newsCategory)
        {
            // ensure no other category exists with this title
            var similarCategory = _newsDbContext.NewsCategories
                .FirstOrDefault(x =>
                    x.Id != newsCategory.Id
                    && x.Title == newsCategory.Title 
                    && x.UserId == newsCategory.UserId
                );

            if (similarCategory != null)
            {
                throw new Exception($"Cannot update News category. A category named {newsCategory.Title} already exists.");
            }

            // locate and update the existing category
            var existingCategory = _newsDbContext.NewsCategories.FirstOrDefault(x => x.Id == newsCategory.Id);

            if(existingCategory == null)
            {
                throw new Exception($"Cannot update News category. Could not locate category {newsCategory.Id}");
            }

            existingCategory.Title = newsCategory.Title;

            _newsDbContext.SaveChanges();         
        }

        public void DeleteNewsCategory(string newsCategoryId)
        {
            var existingCategory = _newsDbContext.NewsCategories.FirstOrDefault(x => x.Id == newsCategoryId);

            if (existingCategory == null)
                throw new Exception("Cannot delete News category. Category does not exist.");

            // remove related data
            var widgetMappings = _newsDbContext.NewsWidgetCategories.Where(x => x.CategoryId == newsCategoryId);
            var newsPostMappings = _newsDbContext.NewsPostCategories.Where(x => x.CategoryId == newsCategoryId);

            _newsDbContext.NewsWidgetCategories.RemoveRange(widgetMappings);
            _newsDbContext.NewsPostCategories.RemoveRange(newsPostMappings);

            // remove the category
            _newsDbContext.NewsCategories.Remove(existingCategory);
            _newsDbContext.SaveChanges();
        }

        /// <summary>
        /// Creates a new NewsPost with scaffolded content
        /// </summary>
        public NewsPost CreateNewsPost(string userId)
        {
            var newsPost = new NewsPost() { Id = KeyGen.NewGuid() };

            // Default Settings
            newsPost.Title = "New News Post";
            newsPost.Excerp = "Enter a brief description about your news...";
            newsPost.Image = "/img/seedimages/news-1.jpeg";
            newsPost.Posted = DateTime.Now;
            newsPost.UserId = userId;
            newsPost.Published = false;
         
            // Create a new content version and add the News post model
            var versionInfo = _contentManager.CreateDraftVersion(CONTENT_TYPE_NEWSPOST, newsPost.Id, userId).Result;

             
            // Create a new contrent tree for this version and use tree builder to insert some content
            var contentTree = _contentManager.CreateContentTree(versionInfo).Result;
            var treeBuilder = _contentManager.CreateTreeBuilder(contentTree);

            treeBuilder.AddRootContent("post-body", settings => {
                settings.WidgetType = "html";
                settings.ModelName = "lorem-ipsum"; // from Angelo.Connect.Web.Data.Json, html.json
            });

            treeBuilder.AddRootContent("post-body", settings => {
                settings.WidgetType = "image";
                settings.ModelName = "idea-banner"; // from Angelo.Connect.Web.Data.Json, image.json
            });

            // save the tree
            treeBuilder.SaveChanges();

            // save the model
            newsPost.VersionCode = versionInfo.VersionCode;
            newsPost.Status = ContentStatus.Scratch; // versionInfo.Status;
            newsPost.ContentTreeId = contentTree.Id;

            _newsDbContext.NewsPosts.Add(newsPost);
            _newsDbContext.SaveChanges();


            var versionData = ExtractVersionData(newsPost);
            _contentManager.SetVersionModelData(versionInfo, versionData).Wait();

            return newsPost;
        }


        public NewsPost CloneNewsPostAsDraft(NewsPost newsPost, string userId)
        {
            var oldVersionInfo = _contentManager.GetVersionInfo(CONTENT_TYPE_NEWSPOST, newsPost.Id, newsPost.VersionCode).Result;

            if (oldVersionInfo == null)
                throw new Exception($"Cannot clone NewsPost. NewsPost {newsPost.Id}, version {newsPost.VersionCode} does not exist.");

            var versionInfo = _contentManager.CreateDraftVersion(CONTENT_TYPE_NEWSPOST, newsPost.Id, userId).Result;
            var clonedContentTree = _contentManager.CloneContentTree(newsPost.ContentTreeId, versionInfo.VersionCode).Result;
            var clonedNewsPost = newsPost.Clone();

            clonedNewsPost.Status = versionInfo.Status;
            clonedNewsPost.VersionCode = versionInfo.VersionCode;
            clonedNewsPost.ContentTreeId = clonedContentTree.Id;

            var versionData = ExtractVersionData(clonedNewsPost);
            _contentManager.SetVersionModelData(versionInfo, versionData).Wait();

            return clonedNewsPost;
        }

        public void PublishNewsPost(string newsPostId, string versionCode)
        {
            // Get the draft version info 
            var versionInfo = _contentManager.GetVersionInfo(CONTENT_TYPE_NEWSPOST, newsPostId, versionCode).Result;

            if(versionInfo == null)
                throw new Exception($"Cannot publish NewsPost. NewsPost {newsPostId} does not have a draft version to publish.");

            // Merge in the version data, set status, & update
            var versionData = _contentManager.GetStoredData<NewsPost>(versionInfo);
            var newsPost = _newsDbContext.NewsPosts.FirstOrDefault(x => x.Id == newsPostId);

            MergeVersionData(newsPost, versionData);

            newsPost.Posted = DateTime.Now;
            newsPost.Published = true;
            newsPost.Status = ContentStatus.Published;
            //entityModel.Active = true;

            _newsDbContext.SaveChanges();

            // Publish the new version / archive old version
            _contentManager.PublishDraftVersion(versionInfo).Wait();

            versionData = ExtractVersionData(newsPost);
            _contentManager.SetVersionModelData(versionInfo, versionData).Wait();
        }

        public void UpdateNewsPost(NewsPost newsPost)
        {
            var versionInfo = _contentManager.GetVersionInfo(CONTENT_TYPE_NEWSPOST, newsPost.Id, newsPost.VersionCode).Result;

            if (versionInfo == null)
                throw new Exception($"Cannot update NewsPost. NewsPost {newsPost.Id} does not have a draft version to edit.");

            if (versionInfo.Status == ContentStatus.Published)
                throw new Exception("Cannot update a published NewsPost");

            var newsPostCurrent = GetNewsPost(newsPost.Id);
            var versionData = _contentManager.GetStoredData<NewsPost>(versionInfo);

            versionData.Title = newsPost.Title;
            versionData.Excerp = newsPost.Excerp;
            versionData.Image = newsPost.Image;
            versionData.Caption = newsPost.Caption;
            versionData.Content = newsPost.Content;
            versionData.Posted = DateTime.Now;
            versionData.Status = ContentStatus.Draft;

            if (newsPostCurrent.VersionCode == versionData.VersionCode)
            {
                // also update the News's phyical copy if version is the same
                MergeVersionData(newsPostCurrent, versionData);

                _newsDbContext.SaveChanges();
            }

            // persist the content model in the version data
            _contentManager.SetVersionModelData(versionInfo, versionData).Wait();
        }

        public void UpdateNewsPostSettings(string newsPostId, bool isPrivate, bool commentsAllowed = false, bool commentsModerated = false)
        {
            // NOTE: Comment Settings are not versioned, so is okay to update the model directly
            var existingNewsPost = _newsDbContext.NewsPosts.FirstOrDefault(x => x.Id == newsPostId);

            if (existingNewsPost == null)
                throw new NullReferenceException($"Could not update News Post settings: Id {newsPostId} not found");


            existingNewsPost.IsPrivate = isPrivate;

            // TODO - Simplify comments settings - no need for Private vs Public since 
            //        "private" News posts would only be viewed by authorized users anyway
            existingNewsPost.PrivateCommentsAllowed = commentsAllowed;
            existingNewsPost.PublicCommentsAllowed = commentsAllowed;
            existingNewsPost.PublicCommentsModerated = commentsModerated;
            existingNewsPost.PrivateCommentsModerated = commentsModerated;

            _newsDbContext.Update(existingNewsPost);
            _newsDbContext.SaveChanges();
        }

        //TODO: Remove this version - we don't need "Private" vs "Public" comment settings - handled by Post Security
        public void UpdateNewsPostSettings(string newsPostId, bool privateCommentsAllowed, bool privateCommentsModerated, bool publicCommentsAllowed, bool publicCommentsModerated)
        {
            // NOTE: Comment Settings are not versioned, so is okay to update the model directly
            var existingNewsPost = _newsDbContext.NewsPosts.FirstOrDefault(x => x.Id == newsPostId);

            if (existingNewsPost == null)
                throw new NullReferenceException($"Could not update News Post: Id {newsPostId} not found");

            if (privateCommentsAllowed)
            {
                existingNewsPost.PrivateCommentsAllowed = true;
                existingNewsPost.PrivateCommentsModerated = privateCommentsModerated;
            }
            else
            {
                existingNewsPost.PrivateCommentsAllowed = false;
                existingNewsPost.PrivateCommentsModerated = false;
            }

            if (publicCommentsAllowed)
            {
                existingNewsPost.PublicCommentsAllowed = true;
                existingNewsPost.PublicCommentsModerated = publicCommentsModerated;
            }
            else
            {
                existingNewsPost.PublicCommentsAllowed = false;
                existingNewsPost.PublicCommentsModerated = false;
            }

            _newsDbContext.Update(existingNewsPost);
            _newsDbContext.SaveChanges();
        }
        
        public void ClearNewsPostCategories(string postId)
        {
            // Remove old mappings
            _newsDbContext.NewsPostCategories.RemoveRange(
                _newsDbContext.NewsPostCategories.Where(x => x.PostId == postId));

            _newsDbContext.SaveChanges();
        }

        public void AddNewsPostTag(string postId, string tagId)
        {
            if (string.IsNullOrEmpty(postId))
                throw new ArgumentNullException("postId");

            if (string.IsNullOrEmpty(tagId))
                throw new ArgumentNullException("tagId");

            _newsDbContext.NewsPostTags.Add(new NewsPostTag
            {
                PostId = postId,
                TagId = tagId,
            });
        }

        public void ClearNewsPostTags(string postId)
        {
            // Remove old mappings
            _newsDbContext.NewsPostTags.RemoveRange(
                _newsDbContext.NewsPostTags.Where(x => x.PostId == postId));

            _newsDbContext.SaveChanges();
        }

        public void SetNewsPostCategories(string postId, IEnumerable<string> categoryIds)
        {
            // Remove old mappings
            _newsDbContext.NewsPostCategories.RemoveRange(
                _newsDbContext.NewsPostCategories.Where(x => x.PostId == postId));

            _newsDbContext.SaveChanges();

            // Add new mappings
            if (categoryIds != null)
            {
                foreach (var categoryId in categoryIds)
                {
                    if (!string.IsNullOrEmpty(categoryId))
                    {
                        _newsDbContext.NewsPostCategories.Add(new NewsPostCategory
                        {
                            PostId = postId,
                            CategoryId = categoryId,
                        });
                    }
                }
            }

            _newsDbContext.SaveChanges();
        }

        public void SetNewsPostTags(string postId, IEnumerable<string> tagIds)
        {
            // Remove old mappings
            _newsDbContext.NewsPostTags.RemoveRange(
                _newsDbContext.NewsPostTags.Where(x => x.PostId == postId));

            _newsDbContext.SaveChanges();

            // Add new mappings
            if (tagIds != null)
            {
                foreach (var tagId in tagIds)
                {
                    if (!string.IsNullOrEmpty(tagId))
                    {
                        _newsDbContext.NewsPostTags.Add(new NewsPostTag
                        {
                            PostId = postId,
                            TagId = tagId,
                        });
                    }
                }
            }

            _newsDbContext.SaveChanges();
        }

        private NewsPost ExtractVersionData(NewsPost newsPost)
        {
            return new NewsPost
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
        }

        private void MergeVersionData(NewsPost newsPost, NewsPost versionData)
        {
            newsPost.VersionCode = versionData.VersionCode;
            newsPost.Status = versionData.Status;
            newsPost.Title = versionData.Title;
            newsPost.Excerp = versionData.Excerp;
            newsPost.Image = versionData.Image;
            newsPost.Caption = versionData.Caption;
            newsPost.Posted = versionData.Posted;
            newsPost.Content = versionData.Content;
            newsPost.ContentTreeId = versionData.ContentTreeId;
            newsPost.UserId = versionData.UserId;
        }

    }
}
