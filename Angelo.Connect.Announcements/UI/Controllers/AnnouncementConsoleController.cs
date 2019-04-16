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
using Angelo.Connect.Announcement.Security;
using Angelo.Connect.Announcement.Services;
using Angelo.Connect.Announcement.Models;
using Angelo.Connect.Announcement.UI.ViewModels;

namespace Angelo.Connect.Announcement.UI.UserConsole
{
    public class AnnouncementConsoleController : Controller
    {

        private AnnouncementManager _announcementManager;
        private ContentManager _contentManager;
        private AnnouncementQueryService _announcementQueries;
        private AnnouncementSecurityService _announcementSecurity;
        private IContextAccessor<UserContext> _userContextAccessor;

        public AnnouncementConsoleController
        (
            AnnouncementManager announcementManager,
            AnnouncementQueryService announcementQueries,
            AnnouncementSecurityService announcementSecurity,
            ContentManager contentManager,
            IContextAccessor<UserContext> userContextAccessor,
            IContextAccessor<SiteContext> siteContextAccessor
        )
        {
            _announcementManager = announcementManager;
            _announcementQueries = announcementQueries;
            _announcementSecurity = announcementSecurity;
            _contentManager = contentManager;
            _userContextAccessor = userContextAccessor;
        }

        [Authorize]
        [HttpGet, Route("/sys/console/announcement/categories/select")]
        public ActionResult AnnouncementPostCategorySelectForm()
        {
            var user = _userContextAccessor.GetContext();

            var userCategories = _announcementManager.GetAnnouncementCategoriesOwnedByUser(user.UserId);
            var sharedCategories = _announcementManager.GetAnnouncementCategoriesSharedWithUser(user);

            ViewData["UserCategories"] = userCategories;
            ViewData["SharedCategories"] = sharedCategories;


            return PartialView("/UI/Views/Console/Announcements/AnnouncementCategorySelect.cshtml");
        }

        [Authorize]
        [HttpGet, Route("/sys/console/announcement/categories/{id}")]
        public ActionResult AnnouncementPostCategoryEditForm(string id)
        {
            var user = _userContextAccessor.GetContext();
            var model = _announcementManager.GetAnnouncementCategory(id);

            return PartialView("/UI/Views/Console/Announcements/AnnouncementCategoryEdit.cshtml", model);
        }

        [Authorize]
        [HttpGet, Route("/sys/console/announcement/categories/create")]
        public ActionResult AnnouncementPostCategoryCreateForm()
        {
            var user = _userContextAccessor.GetContext();
            var model = new AnnouncementCategory
            {
                UserId = user.UserId
            };

            return PartialView("/UI/Views/Console/Announcements/AnnouncementCategoryCreate.cshtml", model);
        }

        [Authorize]
        [HttpGet, Route("/sys/console/announcement/categories/{id}/posts")]
        public ActionResult AnnouncementPostListByCategory(string id)
        {
            var user = _userContextAccessor.GetContext();
            var model = _announcementQueries.QueryByAuthor(user.UserId, id);

            return PartialView("/UI/Views/Console/Announcements/AnnouncementPostList.cshtml", model);
        }

        [Authorize]
        [HttpGet, Route("/sys/console/announcement/posts")]
        public ActionResult AnnouncementPostListByUser()
        {
            var user = _userContextAccessor.GetContext();
            var model = _announcementQueries.QueryByAuthor(user.UserId);

            return PartialView("/UI/Views/Console/Announcements/AnnouncementPostList.cshtml", model);
        }

        [Authorize]
        [HttpGet, Route("/sys/console/announcement/posts/{id}")]
        public IActionResult AnnouncementPostDetailsView(string id, [FromQuery] string version = null)
        {
            var user = _userContextAccessor.GetContext();
            var announcementPost = _announcementManager.GetAnnouncementPost(id, version);

            if (announcementPost == null)
                return NotFound();

            // Ensure the user can edit the post
            if (!_announcementSecurity.AuthorizeForEdit(announcementPost))
                return Unauthorized();

            var versionInfo = _announcementManager.GetVersionInfo(announcementPost.Id, version);

            ViewData["AnnouncementVersionInfo"] = versionInfo;

            return this.PartialContentView(new ContentBindings(versionInfo)
            {
                ViewPath = "~/UI/Views/Console/Announcements/AnnouncementPostDetails.cshtml",
                ViewModel = announcementPost
            });
        }

        [Authorize]
        [HttpGet, Route("/sys/console/announcement/posts/{id}/edit")]
        public IActionResult AnnouncementPostEditForm(string id, [FromQuery] string version = null)
        {
            var user = _userContextAccessor.GetContext();
            var announcementPost = _announcementManager.GetAnnouncementPost(id, version);
           
            // Ensure the user can edit the post
            if (!_announcementSecurity.AuthorizeForEdit(announcementPost))
                return Unauthorized();

            // If the version specified is not already a draft, then create a new draft version 
            if(announcementPost.Status != Connect.Models.ContentStatus.Draft)
            {
                announcementPost = _announcementManager.CloneAnnouncementPostAsDraft(announcementPost, user.UserId);
            }

            // Create the view model
            var announcementPostViewModel = announcementPost.ProjectTo<AnnouncementPostViewModel>();
            var versionInfo = _contentManager.GetVersionInfo(AnnouncementManager.CONTENT_TYPE_ANNOUNCEMENTPOST, announcementPost.Id, announcementPost.VersionCode).Result;

            announcementPostViewModel.VersionLabel = versionInfo.VersionLabel;
            announcementPostViewModel.PostPrivacyConfig = BuildPostPrivacyOptions(announcementPost);
            announcementPostViewModel.Categories = announcementPost.Categories.Select(x => x.Category);
            

            return this.PartialContentView(new ContentBindings(versionInfo)
            {
                ViewPath = "/UI/Views/Console/Announcements/AnnouncementPostDesign.cshtml",
                ViewModel = announcementPostViewModel,
                Editable = true
            });
        }

        [Authorize]
        [HttpGet, Route("/sys/console/announcement/posts/create")]
        public IActionResult AnnouncementPostCreateForm()
        {
            if (!_announcementSecurity.AuthorizeForCreate())
                return Unauthorized();

            var userContext = _userContextAccessor.GetContext();
            var announcementPost = _announcementManager.CreateAnnouncementPost(userContext.UserId);
            var versionInfo = _contentManager.GetVersionInfo(AnnouncementManager.CONTENT_TYPE_ANNOUNCEMENTPOST, announcementPost.Id, announcementPost.VersionCode).Result;

            // build the view model
            var announcementPostViewModel = announcementPost.ProjectTo<AnnouncementPostViewModel>();

            announcementPostViewModel.VersionLabel = versionInfo.VersionLabel;
            announcementPostViewModel.PostPrivacyConfig = BuildPostPrivacyOptions(announcementPost);
            announcementPostViewModel.Categories = new AnnouncementCategory[] { };

            return this.PartialContentView(new ContentBindings(versionInfo)
            {
                ViewPath = "/UI/Views/Console/Announcements/AnnouncementPostDesign.cshtml",
                ViewModel = announcementPostViewModel,
                Editable = true
            });
        }
      
        [Authorize]
        [HttpGet, Route("/sys/console/announcement/posts/{id}/settings")]
        public IActionResult AnnouncementPostSettingsForm(string id)
        {
            var user = _userContextAccessor.GetContext();
            var announcementPost = _announcementManager.GetAnnouncementPost(id);
            
            // Ensure the user can edit the post
            if (!_announcementSecurity.AuthorizeForEdit(announcementPost))
                return Unauthorized();

            var categoryMap = _announcementManager.GetAnnouncementPostCategories(announcementPost.Id);
            var categories = categoryMap.Select(x => new AnnouncementCategory
            {
                Id = x.Category.Id,
                Title = x.Category.Title,
                UserId = x.Category.UserId,
                IsActive = x.Category.IsActive
            });

            var model = new AnnouncementPostSettingsViewModel {
                AnnouncementPostId = announcementPost.Id,
                Categories = categories,
                Versions = _announcementManager.GetVersionHistory(announcementPost.Id),
                PostPrivacyConfig = BuildPostPrivacyOptions(announcementPost),
                IsPrivate = announcementPost.IsPrivate
            };

            return View("/UI/Views/Console/Announcements/AnnouncementPostSettings.cshtml", model);
        }
 

        private IEnumerable<SecurityClaimConfig> BuildPostPrivacyOptions(AnnouncementPost post)
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
                    Claim = new Claim(AnnouncementClaimTypes.AnnouncementPostRead, post.Id),
                    ResourceType = typeof(Angelo.Connect.Announcement.Models.AnnouncementPost).ToString()
                }
            };
        }

    }
}
