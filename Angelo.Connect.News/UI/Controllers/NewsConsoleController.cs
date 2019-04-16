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
using Angelo.Connect.News.Security;
using Angelo.Connect.News.Services;
using Angelo.Connect.News.Models;
using Angelo.Connect.News.UI.ViewModels;

namespace Angelo.Connect.News.UI.UserConsole
{
    public class NewsConsoleController : Controller
    {

        private NewsManager _newsManager;
        private ContentManager _contentManager;
        private NewsQueryService _newsQueries;
        private NewsSecurityService _newsSecurity;
        private IContextAccessor<UserContext> _userContextAccessor;

        public NewsConsoleController
        (
            NewsManager newsManager,
            NewsQueryService newsQueries,
            NewsSecurityService newsSecurity,
            ContentManager contentManager,
            IContextAccessor<UserContext> userContextAccessor,
            IContextAccessor<SiteContext> siteContextAccessor
        )
        {
            _newsManager = newsManager;
            _newsQueries = newsQueries;
            _newsSecurity = newsSecurity;
            _contentManager = contentManager;
            _userContextAccessor = userContextAccessor;
        }

        [Authorize]
        [HttpGet, Route("/sys/console/news/categories/select")]
        public ActionResult NewsPostCategorySelectForm()
        {
            var user = _userContextAccessor.GetContext();

            var userCategories = _newsManager.GetNewsCategoriesOwnedByUser(user.UserId);
            var sharedCategories = _newsManager.GetNewsCategoriesSharedWithUser(user);

            ViewData["UserCategories"] = userCategories;
            ViewData["SharedCategories"] = sharedCategories;


            return PartialView("/UI/Views/Console/News/newsCategorySelect.cshtml");
        }

        [Authorize]
        [HttpGet, Route("/sys/console/news/categories/{id}")]
        public ActionResult NewsPostCategoryEditForm(string id)
        {
            var user = _userContextAccessor.GetContext();
            var model = _newsManager.GetNewsCategory(id);

            return PartialView("/UI/Views/Console/News/newsCategoryEdit.cshtml", model);
        }

        [Authorize]
        [HttpGet, Route("/sys/console/news/categories/create")]
        public ActionResult NewsPostCategoryCreateForm()
        {
            var user = _userContextAccessor.GetContext();
            var model = new NewsCategory
            {
                UserId = user.UserId
            };

            return PartialView("/UI/Views/Console/News/newsCategoryCreate.cshtml", model);
        }

        [Authorize]
        [HttpGet, Route("/sys/console/news/categories/{id}/posts")]
        public ActionResult NewsPostListByCategory(string id)
        {
            var user = _userContextAccessor.GetContext();
            var model = _newsQueries.QueryByAuthor(user.UserId, id);

            return PartialView("/UI/Views/Console/News/newsPostList.cshtml", model);
        }

        [Authorize]
        [HttpGet, Route("/sys/console/news/posts")]
        public ActionResult NewsPostListByUser()
        {
            var user = _userContextAccessor.GetContext();
            var model = _newsQueries.QueryByAuthor(user.UserId);

            return PartialView("/UI/Views/Console/News/newsPostList.cshtml", model);
        }

        [Authorize]
        [HttpGet, Route("/sys/console/news/posts/{id}")]
        public IActionResult NewsPostDetailsView(string id, [FromQuery] string version = null)
        {
            var user = _userContextAccessor.GetContext();
            var newsPost = _newsManager.GetNewsPost(id, version);

            if (newsPost == null)
                return NotFound();

            // Ensure the user can edit the post
            if (!_newsSecurity.AuthorizeForEdit(newsPost))
                return Unauthorized();

            var versionInfo = _newsManager.GetVersionInfo(newsPost.Id, version);

            ViewData["NewsVersionInfo"] = versionInfo;

            return this.PartialContentView(new ContentBindings(versionInfo)
            {
                ViewPath = "~/UI/Views/Console/News/newsPostDetails.cshtml",
                ViewModel = newsPost
            });
        }

        [Authorize]
        [HttpGet, Route("/sys/console/news/posts/{id}/edit")]
        public IActionResult NewsPostEditForm(string id, [FromQuery] string version = null)
        {
            var user = _userContextAccessor.GetContext();
            var newsPost = _newsManager.GetNewsPost(id, version);
           
            // Ensure the user can edit the post
            if (!_newsSecurity.AuthorizeForEdit(newsPost))
                return Unauthorized();

            // If the version specified is not already a draft, then create a new draft version 
            if(newsPost.Status != Connect.Models.ContentStatus.Draft)
            {
                newsPost = _newsManager.CloneNewsPostAsDraft(newsPost, user.UserId);
            }

            // Create the view model
            var newsPostViewModel = newsPost.ProjectTo<NewsPostViewModel>();
            var versionInfo = _contentManager.GetVersionInfo(NewsManager.CONTENT_TYPE_NEWSPOST, newsPost.Id, newsPost.VersionCode).Result;

            newsPostViewModel.VersionLabel = versionInfo.VersionLabel;
            newsPostViewModel.PostPrivacyConfig = BuildPostPrivacyOptions(newsPost);
            newsPostViewModel.Categories = newsPost.Categories.Select(x => x.Category);
            

            return this.PartialContentView(new ContentBindings(versionInfo)
            {
                ViewPath = "/UI/Views/Console/News/newsPostDesign.cshtml",
                ViewModel = newsPostViewModel,
                Editable = true
            });
        }

        [Authorize]
        [HttpGet, Route("/sys/console/news/posts/create")]
        public IActionResult NewsPostCreateForm()
        {
            if (!_newsSecurity.AuthorizeForCreate())
                return Unauthorized();

            var userContext = _userContextAccessor.GetContext();
            var newsPost = _newsManager.CreateNewsPost(userContext.UserId);
            var versionInfo = _contentManager.GetVersionInfo(NewsManager.CONTENT_TYPE_NEWSPOST, newsPost.Id, newsPost.VersionCode).Result;

            // build the view model
            var newsPostViewModel = newsPost.ProjectTo<NewsPostViewModel>();

            newsPostViewModel.VersionLabel = versionInfo.VersionLabel;
            newsPostViewModel.PostPrivacyConfig = BuildPostPrivacyOptions(newsPost);
            newsPostViewModel.Categories = new NewsCategory[] { };

            return this.PartialContentView(new ContentBindings(versionInfo)
            {
                ViewPath = "/UI/Views/Console/News/newsPostDesign.cshtml",
                ViewModel = newsPostViewModel,
                Editable = true
            });
        }
      
        [Authorize]
        [HttpGet, Route("/sys/console/news/posts/{id}/settings")]
        public IActionResult NewsPostSettingsForm(string id)
        {
            var user = _userContextAccessor.GetContext();
            var newsPost = _newsManager.GetNewsPost(id);
            
            // Ensure the user can edit the post
            if (!_newsSecurity.AuthorizeForEdit(newsPost))
                return Unauthorized();

            var categoryMap = _newsManager.GetNewsPostCategories(newsPost.Id);
            var categories = categoryMap.Select(x => new NewsCategory
            {
                Id = x.Category.Id,
                Title = x.Category.Title,
                UserId = x.Category.UserId,
                IsActive = x.Category.IsActive
            });

            var model = new NewsPostSettingsViewModel {
                NewsPostId = newsPost.Id,
                Categories = categories,
                Versions = _newsManager.GetVersionHistory(newsPost.Id),
                PostPrivacyConfig = BuildPostPrivacyOptions(newsPost),
                IsPrivate = newsPost.IsPrivate
            };

            return View("/UI/Views/Console/News/newsPostSettings.cshtml", model);
        }
 

        private IEnumerable<SecurityClaimConfig> BuildPostPrivacyOptions(NewsPost post)
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
                    Claim = new Claim(NewsClaimTypes.NewsPostRead, post.Id),
                    ResourceType = typeof(NewsPost).ToString()
                }
            };
        }

    }
}
