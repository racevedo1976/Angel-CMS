using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Extensions;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Announcement.Data;
using Angelo.Connect.Announcement.Models;
using Angelo.Connect.Announcement.Security;
using Angelo.Connect.Security;
using Angelo.Connect.Services;
using Angelo.Connect.Widgets;
using Angelo.Connect.Models;

namespace Angelo.Connect.Announcement.Services
{
    public class AnnouncementManager
    {
        public const string CONTENT_TYPE_ANNOUNCEMENTPOST = "AnnouncementPost";

        private AnnouncementDbContext _announcementDbContext;
        private ContentManager _contentManager;
        private WidgetProvider _widgetProvider;
        private IContextAccessor<UserContext> _userContextAccessor;

        public AnnouncementManager
        (
            IContextAccessor<UserContext> userContextAccessor,
            ContentManager contentManger,
            WidgetProvider widgetProvider,
            AnnouncementDbContext announcementDbContext
        )
        {
            _userContextAccessor = userContextAccessor;

            _contentManager = contentManger;
            _widgetProvider = widgetProvider;
            _announcementDbContext = announcementDbContext;
        }

        public AnnouncementCategory GetAnnouncementCategory(string categoryId)
        {
            return _announcementDbContext.AnnouncementCategories.FirstOrDefault(x => x.Id == categoryId);
        }

        // TODO: Replace this version with verison that requires userContext
        //       Since ownership may be claims based 
        public List<AnnouncementCategory> GetAnnouncementCategoriesOwnedByUser(string userId)
        {
            var categories = _announcementDbContext.AnnouncementCategories
                .AsNoTracking()
                .Where(x => x.UserId == userId).ToList()
                .OrderBy(x => x.Title)
                .ToList();

            // Ensure navigation properties are null since not tracking 
            categories.ForEach(x => {
                x.AnnouncementPostMap = null;
                x.AnnouncementWidgetMap = null;
            });

            return categories;
        }

        public List<AnnouncementCategory> GetAnnouncementCategoriesSharedWithUser(UserContext userContext)
        {
            // TODO: Filter this list based on User's Claims
            var allowedIds = userContext.SecurityClaims       
                .Where(x => x.Type == AnnouncementClaimTypes.AnnouncementCategoryContribute)
                .Select(x => x.Value)
                .ToList();

            var categories = _announcementDbContext.AnnouncementCategories
                .AsNoTracking()
                .Where(x => allowedIds.Contains(x.Id)).ToList()
                .OrderBy(x => x.Title)
                .ToList();

            // Ensure navigation properties are null since not tracking 
            categories.ForEach(x => {
                x.AnnouncementPostMap = null;
                x.AnnouncementWidgetMap = null;
            });

            return categories;
        }

        public List<AnnouncementPost> GetAnnouncementPostsOwnedByUser(string userId)
        {
            return _announcementDbContext.AnnouncementPosts
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Posted)
                .ThenBy(x => x.Title)
                .ToList();
        }

        public List<AnnouncementPostTag> GetAnnouncementPostTags(string announcementPostId)
        {
            return _announcementDbContext.AnnouncementPostTags
                .Where(x => x.PostId == announcementPostId)
                .ToList();
        }

        public List<AnnouncementPostCategory> GetAnnouncementPostCategories(string announcementPostId)
        {
            return _announcementDbContext.AnnouncementPostCategories
                .Include(x => x.Category)
                .Where(x => x.PostId == announcementPostId)
                .ToList();
        }

        public AnnouncementPost GetAnnouncementPost(string announcementPostId)
        {
            return _announcementDbContext
                .AnnouncementPosts
                .Include(x => x.Categories)
                .ThenInclude(y => y.Category)
                .FirstOrDefault(x => x.Id == announcementPostId);
        }

        public AnnouncementPost GetAnnouncementPost(string announcementPostId, string versionCode)
        {
            var announcementPostCurrent = GetAnnouncementPost(announcementPostId);

            if (versionCode == null || versionCode == announcementPostCurrent.VersionCode)
                return announcementPostCurrent;

            // otherwise get the data stored along with the version
            var versionInfo = _contentManager.GetVersionInfo(CONTENT_TYPE_ANNOUNCEMENTPOST, announcementPostId, versionCode).Result;

            if (versionInfo == null)
                throw new Exception($"Could not get announcement post. Missing version {versionCode}.");

            var announcementPost = announcementPostCurrent.Clone();
            var versionData = _contentManager.GetStoredData<AnnouncementPost>(versionInfo);

            MergeVersionData(announcementPost, versionData);
            
            return announcementPost;
        }

        public ContentVersion GetVersionInfo(string announcementPostId, string versionCode = null)
        {
            var announcementPost = GetAnnouncementPost(announcementPostId);

            if (announcementPost == null)
                return null;

            if(versionCode != null)
                return _contentManager.GetVersionInfo(AnnouncementManager.CONTENT_TYPE_ANNOUNCEMENTPOST, announcementPost.Id, versionCode).Result;
           
            // else
            return announcementPost.Published
                ? _contentManager.GetPublishedVersionInfo(AnnouncementManager.CONTENT_TYPE_ANNOUNCEMENTPOST, announcementPost.Id).Result
                : _contentManager.GetLatestDraftVersionInfo(AnnouncementManager.CONTENT_TYPE_ANNOUNCEMENTPOST, announcementPost.Id).Result;        
        }

        public IEnumerable<ContentVersion> GetVersionHistory(string announcementPostId)
        {
            return _contentManager.GetVersionHistory(CONTENT_TYPE_ANNOUNCEMENTPOST, announcementPostId).Result;
        }

        public void DeleteAnnouncementPost(AnnouncementPost announcementPost)
        {
            // remove content records
            _contentManager.DeleteAllVersions(CONTENT_TYPE_ANNOUNCEMENTPOST, announcementPost.Id).Wait();
            _contentManager.DeleteAllContentTrees(CONTENT_TYPE_ANNOUNCEMENTPOST, announcementPost.Id).Wait();

            // remove child associations
            ClearAnnouncementPostCategories(announcementPost.Id);
            ClearAnnouncementPostTags(announcementPost.Id);

            // remove the post
            _announcementDbContext.AnnouncementPosts.Remove(announcementPost);
            _announcementDbContext.SaveChanges();
        }
      
        public void CreateAnnouncementCategory(AnnouncementCategory announcementCategory)
        {
            // ensure no other category exists with this title
            var similarCategory = _announcementDbContext.AnnouncementCategories
                .FirstOrDefault(x => 
                    x.Title == announcementCategory.Title 
                    && x.UserId == announcementCategory.UserId
                );

            if (similarCategory != null)
            {
                throw new Exception($"Cannot create announcement category. A category named {announcementCategory.Title} already exists.");
            }

            // create the new category
            announcementCategory.Id = KeyGen.NewGuid();

            _announcementDbContext.AnnouncementCategories.Add(announcementCategory);
            _announcementDbContext.SaveChanges();

        }

        public void UpdateAnnouncementCategory(AnnouncementCategory announcementCategory)
        {
            // ensure no other category exists with this title
            var similiarCategory = _announcementDbContext.AnnouncementCategories
                .FirstOrDefault(x =>
                    x.Id != announcementCategory.Id
                    && x.Title == announcementCategory.Title 
                    && x.UserId == announcementCategory.UserId
                );

            if (similiarCategory != null)
            {
                throw new Exception($"Cannot update announcement category. A category named {announcementCategory.Title} already exists.");
            }

            // locate and update the existing category
            var existingCategory = _announcementDbContext.AnnouncementCategories.FirstOrDefault(x => x.Id == announcementCategory.Id);

            if(existingCategory == null)
            {
                throw new Exception($"Cannot update announcement category. Could not locate category {announcementCategory.Id}");
            }

            existingCategory.Title = announcementCategory.Title;

            _announcementDbContext.SaveChanges();         
        }

        public void DeleteAnnouncementCategory(string announcementCategoryId)
        {
            var existingCategory = _announcementDbContext.AnnouncementCategories.FirstOrDefault(x => x.Id == announcementCategoryId);

            if (existingCategory == null)
                throw new Exception("Cannot delete announcement category. Category does not exist.");

            // remove related data
            var widgetMappings = _announcementDbContext.AnnouncementWidgetCategories.Where(x => x.CategoryId == announcementCategoryId);
            var announcementPostMappings = _announcementDbContext.AnnouncementPostCategories.Where(x => x.CategoryId == announcementCategoryId);

            _announcementDbContext.AnnouncementWidgetCategories.RemoveRange(widgetMappings);
            _announcementDbContext.AnnouncementPostCategories.RemoveRange(announcementPostMappings);

            // remove the category
            _announcementDbContext.AnnouncementCategories.Remove(existingCategory);
            _announcementDbContext.SaveChanges();
        }

        /// <summary>
        /// Creates a new AnnouncementPost with scaffolded content
        /// </summary>
        public AnnouncementPost CreateAnnouncementPost(string userId)
        {
            var announcementPost = new AnnouncementPost() { Id = KeyGen.NewGuid() };

            // Default Settings
            announcementPost.Title = "New Announcement Post";
            announcementPost.Excerp = "Enter a brief description about your announcement...";
            announcementPost.Image = "/img/seedimages/announcement-1.jpg";
            announcementPost.Posted = DateTime.Now;
            announcementPost.UserId = userId;
            announcementPost.Published = false;
         
            // Create a new content version and add the announcement post model
            var versionInfo = _contentManager.CreateDraftVersion(CONTENT_TYPE_ANNOUNCEMENTPOST, announcementPost.Id, userId).Result;

             
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
            announcementPost.VersionCode = versionInfo.VersionCode;
            announcementPost.Status = ContentStatus.Scratch; // versionInfo.Status;
            announcementPost.ContentTreeId = contentTree.Id;

            _announcementDbContext.AnnouncementPosts.Add(announcementPost);
            _announcementDbContext.SaveChanges();


            var versionData = ExtractVersionData(announcementPost);
            _contentManager.SetVersionModelData(versionInfo, versionData).Wait();

            return announcementPost;
        }


        public AnnouncementPost CloneAnnouncementPostAsDraft(AnnouncementPost announcementPost, string userId)
        {
            var oldVersionInfo = _contentManager.GetVersionInfo(CONTENT_TYPE_ANNOUNCEMENTPOST, announcementPost.Id, announcementPost.VersionCode).Result;

            if (oldVersionInfo == null)
                throw new Exception($"Cannot clone AnnouncementPost. AnnouncementPost {announcementPost.Id}, version {announcementPost.VersionCode} does not exist.");

            var versionInfo = _contentManager.CreateDraftVersion(CONTENT_TYPE_ANNOUNCEMENTPOST, announcementPost.Id, userId).Result;
            var clonedContentTree = _contentManager.CloneContentTree(announcementPost.ContentTreeId, versionInfo.VersionCode).Result;
            var clonedAnnouncementPost = announcementPost.Clone();

            clonedAnnouncementPost.Status = versionInfo.Status;
            clonedAnnouncementPost.VersionCode = versionInfo.VersionCode;
            clonedAnnouncementPost.ContentTreeId = clonedContentTree.Id;

            var versionData = ExtractVersionData(clonedAnnouncementPost);
            _contentManager.SetVersionModelData(versionInfo, versionData).Wait();

            return clonedAnnouncementPost;
        }

        public void PublishAnnouncementPost(string announcementPostId, string versionCode)
        {
            // Get the draft version info 
            var versionInfo = _contentManager.GetVersionInfo(CONTENT_TYPE_ANNOUNCEMENTPOST, announcementPostId, versionCode).Result;

            if(versionInfo == null)
                throw new Exception($"Cannot publish AnnouncementPost. AnnouncementPost {announcementPostId} does not have a draft version to publish.");

            // Merge in the version data, set status, & update
            var versionData = _contentManager.GetStoredData<AnnouncementPost>(versionInfo);
            var announcementPost = _announcementDbContext.AnnouncementPosts.FirstOrDefault(x => x.Id == announcementPostId);

            MergeVersionData(announcementPost, versionData);

            announcementPost.Posted = DateTime.Now;
            announcementPost.Published = true;
            announcementPost.Status = ContentStatus.Published;
            //entityModel.Active = true;

            _announcementDbContext.SaveChanges();

            // Publish the new version / archive old version
            _contentManager.PublishDraftVersion(versionInfo).Wait();

            versionData = ExtractVersionData(announcementPost);
            _contentManager.SetVersionModelData(versionInfo, versionData).Wait();
        }

        public void UpdateAnnouncementPost(AnnouncementPost announcementPost)
        {
            var versionInfo = _contentManager.GetVersionInfo(CONTENT_TYPE_ANNOUNCEMENTPOST, announcementPost.Id, announcementPost.VersionCode).Result;

            if (versionInfo == null)
                throw new Exception($"Cannot update AnnouncementPost. AnnouncementPost {announcementPost.Id} does not have a draft version to edit.");

            if (versionInfo.Status == ContentStatus.Published)
                throw new Exception("Cannot update a published AnnouncementPost");

            var announcementPostCurrent = GetAnnouncementPost(announcementPost.Id);
            var versionData = _contentManager.GetStoredData<AnnouncementPost>(versionInfo);

            versionData.Title = announcementPost.Title;
            versionData.Excerp = announcementPost.Excerp;
            versionData.Image = announcementPost.Image;
            versionData.Caption = announcementPost.Caption;
            versionData.Content = announcementPost.Content;
            versionData.Posted = DateTime.Now;
            versionData.Status = ContentStatus.Draft;

            if (announcementPostCurrent.VersionCode == versionData.VersionCode)
            {
                // also update the announcement's phyical copy if version is the same
                MergeVersionData(announcementPostCurrent, versionData);

                _announcementDbContext.SaveChanges();
            }

            // persist the content model in the version data
            _contentManager.SetVersionModelData(versionInfo, versionData).Wait();
        }

        public void UpdateAnnouncementPostSettings(string announcementPostId, bool isPrivate, bool commentsAllowed = false, bool commentsModerated = false)
        {
            // NOTE: Comment Settings are not versioned, so is okay to update the model directly
            var existingAnnouncementPost = _announcementDbContext.AnnouncementPosts.FirstOrDefault(x => x.Id == announcementPostId);

            if (existingAnnouncementPost == null)
                throw new NullReferenceException($"Could not update Announcement Post settings: Id {announcementPostId} not found");


            existingAnnouncementPost.IsPrivate = isPrivate;

            // TODO - Simplify comments settings - no need for Private vs Public since 
            //        "private" announcement posts would only be viewed by authorized users anyway
            existingAnnouncementPost.PrivateCommentsAllowed = commentsAllowed;
            existingAnnouncementPost.PublicCommentsAllowed = commentsAllowed;
            existingAnnouncementPost.PublicCommentsModerated = commentsModerated;
            existingAnnouncementPost.PrivateCommentsModerated = commentsModerated;

            _announcementDbContext.Update(existingAnnouncementPost);
            _announcementDbContext.SaveChanges();
        }

        //TODO: Remove this version - we don't need "Private" vs "Public" comment settings - handled by Post Security
        public void UpdateAnnouncementPostSettings(string announcementPostId, bool privateCommentsAllowed, bool privateCommentsModerated, bool publicCommentsAllowed, bool publicCommentsModerated)
        {
            // NOTE: Comment Settings are not versioned, so is okay to update the model directly
            var existingAnnouncementPost = _announcementDbContext.AnnouncementPosts.FirstOrDefault(x => x.Id == announcementPostId);

            if (existingAnnouncementPost == null)
                throw new NullReferenceException($"Could not update Announcement Post: Id {announcementPostId} not found");

            if (privateCommentsAllowed)
            {
                existingAnnouncementPost.PrivateCommentsAllowed = true;
                existingAnnouncementPost.PrivateCommentsModerated = privateCommentsModerated;
            }
            else
            {
                existingAnnouncementPost.PrivateCommentsAllowed = false;
                existingAnnouncementPost.PrivateCommentsModerated = false;
            }

            if (publicCommentsAllowed)
            {
                existingAnnouncementPost.PublicCommentsAllowed = true;
                existingAnnouncementPost.PublicCommentsModerated = publicCommentsModerated;
            }
            else
            {
                existingAnnouncementPost.PublicCommentsAllowed = false;
                existingAnnouncementPost.PublicCommentsModerated = false;
            }

            _announcementDbContext.Update(existingAnnouncementPost);
            _announcementDbContext.SaveChanges();
        }
        
        public void ClearAnnouncementPostCategories(string postId)
        {
            // Remove old mappings
            _announcementDbContext.AnnouncementPostCategories.RemoveRange(
                _announcementDbContext.AnnouncementPostCategories.Where(x => x.PostId == postId));

            _announcementDbContext.SaveChanges();
        }

        public void AddAnnouncementPostTag(string postId, string tagId)
        {
            if (string.IsNullOrEmpty(postId))
                throw new ArgumentNullException("postId");

            if (string.IsNullOrEmpty(tagId))
                throw new ArgumentNullException("tagId");

            _announcementDbContext.AnnouncementPostTags.Add(new AnnouncementPostTag
            {
                PostId = postId,
                TagId = tagId,
            });
        }

        public void ClearAnnouncementPostTags(string postId)
        {
            // Remove old mappings
            _announcementDbContext.AnnouncementPostTags.RemoveRange(
                _announcementDbContext.AnnouncementPostTags.Where(x => x.PostId == postId));

            _announcementDbContext.SaveChanges();
        }

        public void SetAnnouncementPostCategories(string postId, IEnumerable<string> categoryIds)
        {
            // Remove old mappings
            _announcementDbContext.AnnouncementPostCategories.RemoveRange(
                _announcementDbContext.AnnouncementPostCategories.Where(x => x.PostId == postId));

            _announcementDbContext.SaveChanges();

            // Add new mappings
            if (categoryIds != null)
            {
                foreach (var categoryId in categoryIds)
                {
                    if (!string.IsNullOrEmpty(categoryId))
                    {
                        _announcementDbContext.AnnouncementPostCategories.Add(new AnnouncementPostCategory
                        {
                            PostId = postId,
                            CategoryId = categoryId,
                        });
                    }
                }
            }

            _announcementDbContext.SaveChanges();
        }

        public void SetAnnouncementPostTags(string postId, IEnumerable<string> tagIds)
        {
            // Remove old mappings
            _announcementDbContext.AnnouncementPostTags.RemoveRange(
                _announcementDbContext.AnnouncementPostTags.Where(x => x.PostId == postId));

            _announcementDbContext.SaveChanges();

            // Add new mappings
            if (tagIds != null)
            {
                foreach (var tagId in tagIds)
                {
                    if (!string.IsNullOrEmpty(tagId))
                    {
                        _announcementDbContext.AnnouncementPostTags.Add(new AnnouncementPostTag
                        {
                            PostId = postId,
                            TagId = tagId,
                        });
                    }
                }
            }

            _announcementDbContext.SaveChanges();
        }

        private AnnouncementPost ExtractVersionData(AnnouncementPost announcementPost)
        {
            return new AnnouncementPost
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
        }

        private void MergeVersionData(AnnouncementPost announcementPost, AnnouncementPost versionData)
        {
            announcementPost.VersionCode = versionData.VersionCode;
            announcementPost.Status = versionData.Status;
            announcementPost.Title = versionData.Title;
            announcementPost.Excerp = versionData.Excerp;
            announcementPost.Image = versionData.Image;
            announcementPost.Caption = versionData.Caption;
            announcementPost.Posted = versionData.Posted;
            announcementPost.Content = versionData.Content;
            announcementPost.ContentTreeId = versionData.ContentTreeId;
            announcementPost.UserId = versionData.UserId;
        }

    }
}
