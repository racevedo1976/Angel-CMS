using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper.Extensions;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Rendering;
using Angelo.Connect.Security;
using Angelo.Connect.Services;
using Angelo.Connect.Blog.Security;
using Angelo.Connect.Blog.Services;
using Angelo.Connect.Blog.Models;
using Angelo.Connect.Blog.UI.ViewModels;

namespace Angelo.Connect.Blog.UI.UserConsole
{
    public class BlogConsoleController : Controller
    {

        private BlogManager _blogManager;
        private ContentManager _contentManager;
        private BlogQueryService _blogQueries;
        private BlogSecurityService _blogSecurity;
        private IContextAccessor<UserContext> _userContextAccessor;

        public BlogConsoleController
        (
            BlogManager blogManager,
            BlogQueryService blogQueries,
            BlogSecurityService blogSecurity,
            ContentManager contentManager,
            IContextAccessor<UserContext> userContextAccessor,
            IContextAccessor<SiteContext> siteContextAccessor
        )
        {
            _blogManager = blogManager;
            _blogQueries = blogQueries;
            _blogSecurity = blogSecurity;
            _contentManager = contentManager;
            _userContextAccessor = userContextAccessor;
        }

        [Authorize]
        [HttpGet, Route("/sys/console/blog/categories/select")]
        public ActionResult BlogPostCategorySelectForm()
        {
            var user = _userContextAccessor.GetContext();

            var userCategories = _blogManager.GetBlogCategoriesOwnedByUser(user.UserId);
            var sharedCategories = _blogManager.GetBlogCategoriesSharedWithUser(user);

            ViewData["UserCategories"] = userCategories;
            ViewData["SharedCategories"] = sharedCategories;


            return PartialView("/UI/Views/Console/BlogCategorySelect.cshtml");
        }

        [Authorize]
        [HttpGet, Route("/sys/console/blog/categories/{id}")]
        public ActionResult BlogPostCategoryEditForm(string id)
        {
            var user = _userContextAccessor.GetContext();
            var model = _blogManager.GetBlogCategory(id);

            return PartialView("/UI/Views/Console/BlogCategoryEdit.cshtml", model);
        }

        [Authorize]
        [HttpGet, Route("/sys/console/blog/categories/create")]
        public ActionResult BlogPostCategoryCreateForm()
        {
            var user = _userContextAccessor.GetContext();
            var model = new BlogCategory
            {
                UserId = user.UserId
            };

            return PartialView("/UI/Views/Console/BlogCategoryCreate.cshtml", model);
        }

        [Authorize]
        [HttpGet, Route("/sys/console/blog/categories/{id}/posts")]
        public ActionResult BlogPostListByCategory(string id)
        {
            var user = _userContextAccessor.GetContext();
            var model = _blogQueries.QueryByAuthor(user.UserId, id);

            return PartialView("/UI/Views/Console/BlogPostList.cshtml", model);
        }

        [Authorize]
        [HttpGet, Route("/sys/console/blog/posts")]
        public ActionResult BlogPostListByUser()
        {
            var user = _userContextAccessor.GetContext();
            var model = _blogQueries.QueryByAuthor(user.UserId);

            return PartialView("/UI/Views/Console/BlogPostList.cshtml", model);
        }

        [Authorize]
        [HttpGet, Route("/sys/console/blog/posts/{id}")]
        public IActionResult BlogPostDetailsView(string id, [FromQuery] string version = null)
        {
            var user = _userContextAccessor.GetContext();
            var blogPost = _blogManager.GetBlogPost(id, version);

            if (blogPost == null)
                return NotFound();

            // Ensure the user can edit the post
            if (!_blogSecurity.AuthorizeForEdit(blogPost))
                return Unauthorized();

            var versionInfo = _blogManager.GetVersionInfo(blogPost.Id, version);

            ViewData["BlogVersionInfo"] = versionInfo;

            return this.PartialContentView(new ContentBindings(versionInfo)
            {
                ViewPath = "~/UI/Views/Console/BlogPostDetails.cshtml",
                ViewModel = blogPost
            });
        }

        [Authorize]
        [HttpGet, Route("/sys/console/blog/posts/{id}/edit")]
        public IActionResult BlogPostEditForm(string id, [FromQuery] string version = null)
        {
            var user = _userContextAccessor.GetContext();
            var blogPost = _blogManager.GetBlogPost(id, version);
           
            // Ensure the user can edit the post
            if (!_blogSecurity.AuthorizeForEdit(blogPost))
                return Unauthorized();

            // If the version specified is not already a draft, then create a new draft version 
            if(blogPost.Status != Connect.Models.ContentStatus.Draft)
            {
                blogPost = _blogManager.CloneBlogPostAsDraft(blogPost, user.UserId);
            }

            // Create the view model
            var blogPostViewModel = blogPost.ProjectTo<BlogPostViewModel>();
            var versionInfo = _contentManager.GetVersionInfo(BlogManager.CONTENT_TYPE_BLOGPOST, blogPost.Id, blogPost.VersionCode).Result;

            blogPostViewModel.VersionLabel = versionInfo.VersionLabel;
            blogPostViewModel.PostPrivacyConfig = BuildPostPrivacyOptions(blogPost);
            blogPostViewModel.Categories = blogPost.Categories.Select(x => x.Category);
            

            return this.PartialContentView(new ContentBindings(versionInfo)
            {
                ViewPath = "/UI/Views/Console/BlogPostDesign.cshtml",
                ViewModel = blogPostViewModel,
                Editable = true
            });
        }

        [Authorize]
        [HttpGet, Route("/sys/console/blog/posts/create")]
        public IActionResult BlogPostCreateForm()
        {
            if (!_blogSecurity.AuthorizeForCreate())
                return Unauthorized();

            var userContext = _userContextAccessor.GetContext();
            var blogPost = _blogManager.CreateBlogPost(userContext.UserId);
            var versionInfo = _contentManager.GetVersionInfo(BlogManager.CONTENT_TYPE_BLOGPOST, blogPost.Id, blogPost.VersionCode).Result;

            // build the view model
            var blogPostViewModel = blogPost.ProjectTo<BlogPostViewModel>();

            blogPostViewModel.VersionLabel = versionInfo.VersionLabel;
            blogPostViewModel.PostPrivacyConfig = BuildPostPrivacyOptions(blogPost);
            blogPostViewModel.Categories = new BlogCategory[] { };

            return this.PartialContentView(new ContentBindings(versionInfo)
            {
                ViewPath = "/UI/Views/Console/BlogPostDesign.cshtml",
                ViewModel = blogPostViewModel,
                Editable = true
            });
        }
      
        [Authorize]
        [HttpGet, Route("/sys/console/blog/posts/{id}/settings")]
        public IActionResult BlogPostSettingsForm(string id)
        {
            var user = _userContextAccessor.GetContext();
            var blogPost = _blogManager.GetBlogPost(id);
            
            // Ensure the user can edit the post
            if (!_blogSecurity.AuthorizeForEdit(blogPost))
                return Unauthorized();

            var categoryMap = _blogManager.GetBlogPostCategories(blogPost.Id);
            var categories = categoryMap.Select(x => new BlogCategory
            {
                Id = x.Category.Id,
                Title = x.Category.Title,
                UserId = x.Category.UserId,
                IsActive = x.Category.IsActive
            });

            var model = new BlogPostSettingsViewModel {
                BlogPostId = blogPost.Id,
                Categories = categories,
                Versions = _blogManager.GetVersionHistory(blogPost.Id),
                PostPrivacyConfig = BuildPostPrivacyOptions(blogPost),
                IsPrivate = blogPost.IsPrivate
            };

            return View("/UI/Views/Console/BlogPostSettings.cshtml", model);
        }
 

        private IEnumerable<SecurityClaimConfig> BuildPostPrivacyOptions(BlogPost post)
        {
            return new List<SecurityClaimConfig>()
            {
                new SecurityClaimConfig
                {
                    Title = "Post Privacy",
                    Description = "Manage who can view post.",
                    AllowRoles = false,
                    AllowUsers = false,
                    AllowGroups = true,
                    Claim = new Claim(BlogClaimTypes.BlogPostRead, post.Id),
                    ResourceType = typeof(Angelo.Connect.Blog.Models.BlogPost).ToString()
                }
            };
        }

    }
}
