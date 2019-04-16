using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Extensions;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Blog.Data;
using Angelo.Connect.Blog.Models;
using Angelo.Connect.Blog.Security;
using Angelo.Connect.Security;
using Angelo.Connect.Services;
using Angelo.Connect.Widgets;
using Angelo.Connect.Models;

namespace Angelo.Connect.Blog.Services
{
    public class BlogManager
    {
        public const string CONTENT_TYPE_BLOGPOST = "BlogPost";

        private BlogDbContext _blogDbContext;
        private ContentManager _contentManager;
        private WidgetProvider _widgetProvider;
        private IContextAccessor<UserContext> _userContextAccessor;

        public BlogManager
        (
            IContextAccessor<UserContext> userContextAccessor,
            ContentManager contentManger,
            WidgetProvider widgetProvider,
            BlogDbContext blogDbContext
        )
        {
            _userContextAccessor = userContextAccessor;

            _contentManager = contentManger;
            _widgetProvider = widgetProvider;
            _blogDbContext = blogDbContext;
        }

        public BlogCategory GetBlogCategory(string categoryId)
        {
            return _blogDbContext.BlogCategories.FirstOrDefault(x => x.Id == categoryId);
        }

        // TODO: Replace this version with verison that requires userContext
        //       Since ownership may be claims based 
        public List<BlogCategory> GetBlogCategoriesOwnedByUser(string userId)
        {
            var categories = _blogDbContext.BlogCategories
                .AsNoTracking()
                .Where(x => x.UserId == userId).ToList()
                .OrderBy(x => x.Title)
                .ToList();

            // Ensure navigation properties are null since not tracking 
            categories.ForEach(x => {
                x.BlogPostMap = null;
                x.BlogWidgetMap = null;
            });

            return categories;
        }

        public List<BlogCategory> GetBlogCategoriesSharedWithUser(UserContext userContext)
        {
            // TODO: Filter this list based on User's Claims
            var allowedIds = userContext.SecurityClaims       
                .Where(x => x.Type == BlogClaimTypes.BlogCategoryContribute)
                .Select(x => x.Value)
                .ToList();

            var categories = _blogDbContext.BlogCategories
                .AsNoTracking()
                .Where(x => allowedIds.Contains(x.Id)).ToList()
                .OrderBy(x => x.Title)
                .ToList();

            // Ensure navigation properties are null since not tracking 
            categories.ForEach(x => {
                x.BlogPostMap = null;
                x.BlogWidgetMap = null;
            });

            return categories;
        }

        public List<BlogPost> GetBlogPostsOwnedByUser(string userId)
        {
            return _blogDbContext.BlogPosts
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Posted)
                .ThenBy(x => x.Title)
                .ToList();
        }

        public List<BlogPostTag> GetBlogPostTags(string blogPostId)
        {
            return _blogDbContext.BlogPostTags
                .Where(x => x.PostId == blogPostId)
                .ToList();
        }

        public List<BlogPostCategory> GetBlogPostCategories(string blogPostId)
        {
            return _blogDbContext.BlogPostCategories
                .Include(x => x.Category)
                .Where(x => x.PostId == blogPostId)
                .ToList();
        }

        public BlogPost GetBlogPost(string blogPostId)
        {
            return _blogDbContext
                .BlogPosts
                .Include(x => x.Categories)
                .ThenInclude(y => y.Category)
                .FirstOrDefault(x => x.Id == blogPostId);
        }

        public BlogPost GetBlogPost(string blogPostId, string versionCode)
        {
            var blogPostCurrent = GetBlogPost(blogPostId);

            if (versionCode == null || versionCode == blogPostCurrent.VersionCode)
                return blogPostCurrent;

            // otherwise get the data stored along with the version
            var versionInfo = _contentManager.GetVersionInfo(CONTENT_TYPE_BLOGPOST, blogPostId, versionCode).Result;

            if (versionInfo == null)
                throw new Exception($"Could not get blog post. Missing version {versionCode}.");

            var blogPost = blogPostCurrent.Clone();
            var versionData = _contentManager.GetStoredData<BlogPost>(versionInfo);

            MergeVersionData(blogPost, versionData);
            
            return blogPost;
        }

        public ContentVersion GetVersionInfo(string blogPostId, string versionCode = null)
        {
            var blogPost = GetBlogPost(blogPostId);

            if (blogPost == null)
                return null;

            if(versionCode != null)
                return _contentManager.GetVersionInfo(BlogManager.CONTENT_TYPE_BLOGPOST, blogPost.Id, versionCode).Result;
           
            // else
            return blogPost.Published
                ? _contentManager.GetPublishedVersionInfo(BlogManager.CONTENT_TYPE_BLOGPOST, blogPost.Id).Result
                : _contentManager.GetLatestDraftVersionInfo(BlogManager.CONTENT_TYPE_BLOGPOST, blogPost.Id).Result;        
        }

        public IEnumerable<ContentVersion> GetVersionHistory(string blogPostId)
        {
            return _contentManager.GetVersionHistory(CONTENT_TYPE_BLOGPOST, blogPostId).Result;
        }

        public void DeleteBlogPost(BlogPost blogPost)
        {
            // remove content records
            _contentManager.DeleteAllVersions(CONTENT_TYPE_BLOGPOST, blogPost.Id).Wait();
            _contentManager.DeleteAllContentTrees(CONTENT_TYPE_BLOGPOST, blogPost.Id).Wait();

            // remove child associations
            ClearBlogPostCategories(blogPost.Id);
            ClearBlogPostTags(blogPost.Id);

            // remove the post
            _blogDbContext.BlogPosts.Remove(blogPost);
            _blogDbContext.SaveChanges();
        }
      
        public void CreateBlogCategory(BlogCategory blogCategory)
        {
            // ensure no other category exists with this title
            var similarCategory = _blogDbContext.BlogCategories
                .FirstOrDefault(x => 
                    x.Title == blogCategory.Title 
                    && x.UserId == blogCategory.UserId
                );

            if (similarCategory != null)
            {
                throw new Exception($"Cannot create blog category. A category named {blogCategory.Title} already exists.");
            }

            // create the new category
            blogCategory.Id = KeyGen.NewGuid();

            _blogDbContext.BlogCategories.Add(blogCategory);
            _blogDbContext.SaveChanges();

        }

        public void UpdateBlogCategory(BlogCategory blogCategory)
        {
            // ensure no other category exists with this title
            var similiarCategory = _blogDbContext.BlogCategories
                .FirstOrDefault(x =>
                    x.Id != blogCategory.Id
                    && x.Title == blogCategory.Title 
                    && x.UserId == blogCategory.UserId
                );

            if (similiarCategory != null)
            {
                throw new Exception($"Cannot update blog category. A category named {blogCategory.Title} already exists.");
            }

            // locate and update the existing category
            var existingCategory = _blogDbContext.BlogCategories.FirstOrDefault(x => x.Id == blogCategory.Id);

            if(existingCategory == null)
            {
                throw new Exception($"Cannot update blog category. Could not locate category {blogCategory.Id}");
            }

            existingCategory.Title = blogCategory.Title;

            _blogDbContext.SaveChanges();         
        }

        public void DeleteBlogCategory(string blogCategoryId)
        {
            var existingCategory = _blogDbContext.BlogCategories.FirstOrDefault(x => x.Id == blogCategoryId);

            if (existingCategory == null)
                throw new Exception("Cannot delete blog category. Category does not exist.");

            // remove related data
            var widgetMappings = _blogDbContext.BlogWidgetCategories.Where(x => x.CategoryId == blogCategoryId);
            var blogPostMappings = _blogDbContext.BlogPostCategories.Where(x => x.CategoryId == blogCategoryId);

            _blogDbContext.BlogWidgetCategories.RemoveRange(widgetMappings);
            _blogDbContext.BlogPostCategories.RemoveRange(blogPostMappings);

            // remove the category
            _blogDbContext.BlogCategories.Remove(existingCategory);
            _blogDbContext.SaveChanges();
        }

        /// <summary>
        /// Creates a new BlogPost with scaffolded content
        /// </summary>
        public BlogPost CreateBlogPost(string userId)
        {
            var blogPost = new BlogPost() { Id = KeyGen.NewGuid() };

            // Default Settings
            blogPost.Title = "New Post";
            blogPost.Excerp = "Enter a brief description about your blog...";
            blogPost.Image = "/img/seedimages/blogpost.png";
            blogPost.Posted = DateTime.Now;
            blogPost.UserId = userId;
            blogPost.Published = false;
         
            // Create a new content version and add the blog post model
            var versionInfo = _contentManager.CreateDraftVersion(CONTENT_TYPE_BLOGPOST, blogPost.Id, userId).Result;

             
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
            blogPost.VersionCode = versionInfo.VersionCode;
            blogPost.Status = ContentStatus.Scratch; // versionInfo.Status;
            blogPost.ContentTreeId = contentTree.Id;

            _blogDbContext.BlogPosts.Add(blogPost);
            _blogDbContext.SaveChanges();


            var versionData = ExtractVersionData(blogPost);
            _contentManager.SetVersionModelData(versionInfo, versionData).Wait();

            return blogPost;
        }


        public BlogPost CloneBlogPostAsDraft(BlogPost blogPost, string userId)
        {
            var oldVersionInfo = _contentManager.GetVersionInfo(CONTENT_TYPE_BLOGPOST, blogPost.Id, blogPost.VersionCode).Result;

            if (oldVersionInfo == null)
                throw new Exception($"Cannot clone BlogPost. BlogPost {blogPost.Id}, version {blogPost.VersionCode} does not exist.");

            var versionInfo = _contentManager.CreateDraftVersion(CONTENT_TYPE_BLOGPOST, blogPost.Id, userId).Result;
            var clonedContentTree = _contentManager.CloneContentTree(blogPost.ContentTreeId, versionInfo.VersionCode).Result;
            var clonedBlogPost = blogPost.Clone();

            clonedBlogPost.Status = versionInfo.Status;
            clonedBlogPost.VersionCode = versionInfo.VersionCode;
            clonedBlogPost.ContentTreeId = clonedContentTree.Id;

            var versionData = ExtractVersionData(clonedBlogPost);
            _contentManager.SetVersionModelData(versionInfo, versionData).Wait();

            return clonedBlogPost;
        }

        public void PublishBlogPost(string blogPostId, string versionCode)
        {
            // Get the draft version info 
            var versionInfo = _contentManager.GetVersionInfo(CONTENT_TYPE_BLOGPOST, blogPostId, versionCode).Result;

            if(versionInfo == null)
                throw new Exception($"Cannot publish BlogPost. BlogPost {blogPostId} does not have a draft version to publish.");

            // Merge in the version data, set status, & update
            var versionData = _contentManager.GetStoredData<BlogPost>(versionInfo);
            var blogPost = _blogDbContext.BlogPosts.FirstOrDefault(x => x.Id == blogPostId);

            MergeVersionData(blogPost, versionData);

            blogPost.Posted = DateTime.Now;
            blogPost.Published = true;
            blogPost.Status = ContentStatus.Published;
            //entityModel.Active = true;

            _blogDbContext.SaveChanges();

            // Publish the new version / archive old version
            _contentManager.PublishDraftVersion(versionInfo).Wait();

            versionData = ExtractVersionData(blogPost);
            _contentManager.SetVersionModelData(versionInfo, versionData).Wait();
        }

        public void UpdateBlogPost(BlogPost blogPost)
        {
            var versionInfo = _contentManager.GetVersionInfo(CONTENT_TYPE_BLOGPOST, blogPost.Id, blogPost.VersionCode).Result;

            if (versionInfo == null)
                throw new Exception($"Cannot update BlogPost. BlogPost {blogPost.Id} does not have a draft version to edit.");

            if (versionInfo.Status == ContentStatus.Published)
                throw new Exception("Cannot update a published BlogPost");

            var blogPostCurrent = GetBlogPost(blogPost.Id);
            var versionData = _contentManager.GetStoredData<BlogPost>(versionInfo);

            versionData.Title = blogPost.Title;
            versionData.Excerp = blogPost.Excerp;
            versionData.Image = blogPost.Image;
            versionData.Caption = blogPost.Caption;
            versionData.Content = blogPost.Content;
            versionData.Posted = DateTime.Now;
            versionData.Status = ContentStatus.Draft;

            if (blogPostCurrent.VersionCode == versionData.VersionCode)
            {
                // also update the blog's phyical copy if version is the same
                MergeVersionData(blogPostCurrent, versionData);

                _blogDbContext.SaveChanges();
            }

            // persist the content model in the version data
            _contentManager.SetVersionModelData(versionInfo, versionData).Wait();
        }

        public void UpdateBlogPostSettings(string blogPostId, bool isPrivate, bool commentsAllowed = false, bool commentsModerated = false)
        {
            // NOTE: Comment Settings are not versioned, so is okay to update the model directly
            var existingBlogPost = _blogDbContext.BlogPosts.FirstOrDefault(x => x.Id == blogPostId);

            if (existingBlogPost == null)
                throw new NullReferenceException($"Could not update Blog Post settings: Id {blogPostId} not found");


            existingBlogPost.IsPrivate = isPrivate;

            // TODO - Simplify comments settings - no need for Private vs Public since 
            //        "private" blog posts would only be viewed by authorized users anyway
            existingBlogPost.PrivateCommentsAllowed = commentsAllowed;
            existingBlogPost.PublicCommentsAllowed = commentsAllowed;
            existingBlogPost.PublicCommentsModerated = commentsModerated;
            existingBlogPost.PrivateCommentsModerated = commentsModerated;

            _blogDbContext.Update(existingBlogPost);
            _blogDbContext.SaveChanges();
        }

        //TODO: Remove this version - we don't need "Private" vs "Public" comment settings - handled by Post Security
        public void UpdateBlogPostSettings(string blogPostId, bool privateCommentsAllowed, bool privateCommentsModerated, bool publicCommentsAllowed, bool publicCommentsModerated)
        {
            // NOTE: Comment Settings are not versioned, so is okay to update the model directly
            var existingBlogPost = _blogDbContext.BlogPosts.FirstOrDefault(x => x.Id == blogPostId);

            if (existingBlogPost == null)
                throw new NullReferenceException($"Could not update Blog Post: Id {blogPostId} not found");

            if (privateCommentsAllowed)
            {
                existingBlogPost.PrivateCommentsAllowed = true;
                existingBlogPost.PrivateCommentsModerated = privateCommentsModerated;
            }
            else
            {
                existingBlogPost.PrivateCommentsAllowed = false;
                existingBlogPost.PrivateCommentsModerated = false;
            }

            if (publicCommentsAllowed)
            {
                existingBlogPost.PublicCommentsAllowed = true;
                existingBlogPost.PublicCommentsModerated = publicCommentsModerated;
            }
            else
            {
                existingBlogPost.PublicCommentsAllowed = false;
                existingBlogPost.PublicCommentsModerated = false;
            }

            _blogDbContext.Update(existingBlogPost);
            _blogDbContext.SaveChanges();
        }
        
        public void ClearBlogPostCategories(string postId)
        {
            // Remove old mappings
            _blogDbContext.BlogPostCategories.RemoveRange(
                _blogDbContext.BlogPostCategories.Where(x => x.PostId == postId));

            _blogDbContext.SaveChanges();
        }

        public void AddBlogPostTag(string postId, string tagId)
        {
            if (string.IsNullOrEmpty(postId))
                throw new ArgumentNullException("postId");

            if (string.IsNullOrEmpty(tagId))
                throw new ArgumentNullException("tagId");

            _blogDbContext.BlogPostTags.Add(new BlogPostTag
            {
                PostId = postId,
                TagId = tagId,
            });
        }

        public void ClearBlogPostTags(string postId)
        {
            // Remove old mappings
            _blogDbContext.BlogPostTags.RemoveRange(
                _blogDbContext.BlogPostTags.Where(x => x.PostId == postId));

            _blogDbContext.SaveChanges();
        }

        public void SetBlogPostCategories(string postId, IEnumerable<string> categoryIds)
        {
            // Remove old mappings
            _blogDbContext.BlogPostCategories.RemoveRange(
                _blogDbContext.BlogPostCategories.Where(x => x.PostId == postId));

            _blogDbContext.SaveChanges();

            // Add new mappings
            if (categoryIds != null)
            {
                foreach (var categoryId in categoryIds)
                {
                    if (!string.IsNullOrEmpty(categoryId))
                    {
                        _blogDbContext.BlogPostCategories.Add(new BlogPostCategory
                        {
                            PostId = postId,
                            CategoryId = categoryId,
                        });
                    }
                }
            }

            _blogDbContext.SaveChanges();
        }

        public void SetBlogPostTags(string postId, IEnumerable<string> tagIds)
        {
            // Remove old mappings
            _blogDbContext.BlogPostTags.RemoveRange(
                _blogDbContext.BlogPostTags.Where(x => x.PostId == postId));

            _blogDbContext.SaveChanges();

            // Add new mappings
            if (tagIds != null)
            {
                foreach (var tagId in tagIds)
                {
                    if (!string.IsNullOrEmpty(tagId))
                    {
                        _blogDbContext.BlogPostTags.Add(new BlogPostTag
                        {
                            PostId = postId,
                            TagId = tagId,
                        });
                    }
                }
            }

            _blogDbContext.SaveChanges();
        }

        private BlogPost ExtractVersionData(BlogPost blogPost)
        {
            return new BlogPost
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
        }

        private void MergeVersionData(BlogPost blogPost, BlogPost versionData)
        {
            blogPost.VersionCode = versionData.VersionCode;
            blogPost.Status = versionData.Status;
            blogPost.Title = versionData.Title;
            blogPost.Excerp = versionData.Excerp;
            blogPost.Image = versionData.Image;
            blogPost.Caption = versionData.Caption;
            blogPost.Posted = versionData.Posted;
            blogPost.Content = versionData.Content;
            blogPost.ContentTreeId = versionData.ContentTreeId;
            blogPost.UserId = versionData.UserId;
        }

    }
}
