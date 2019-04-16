using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

using Angelo.Connect.Configuration;
using Angelo.Connect.Security;
using Angelo.Connect.Services;
using Angelo.Identity;
using Angelo.Identity.Services;
using Angelo.Connect.Models;
using Angelo.Connect.Web.UI.ViewModels.Admin;

namespace Angelo.Connect.Web.UI.Controllers.Admin
{
    public class SiteAdminController : SiteControllerBase
    {
        private SiteManager _siteManager;
        private PageManager _pageManager;
        private DirectoryManager _directoryManager;
        private SecurityPoolManager _poolManager;
        private ClientManager _clientManager;

        public SiteAdminController
        (
            SiteManager siteManager,
            PageManager pageManager,
            SecurityPoolManager poolManager,
            DirectoryManager directoryManager,
            ClientManager clientManager,
            SiteAdminContextAccessor siteContextAccessor,
            IAuthorizationService authorizationService,
            ILogger<SiteAdminController> logger
        )
        : base(siteContextAccessor, logger)
        {
            _siteManager = siteManager;
            _pageManager = pageManager;
            _poolManager = poolManager;            
            _clientManager = clientManager;
            _directoryManager = directoryManager;
        }

        [Authorize(policy: PolicyNames.StubPolicy)]
        public IActionResult Categories()
        {
            ViewData["Level"] = "Site";
            ViewData["OwnerId"] = Site.Id;
            ViewData["SiteId"] = Site.Id;

            // TODO: Refactor view to remove reference to clientId
            ViewData["ClientId"] = null;

            return View();
        }

        [Authorize(policy: PolicyNames.SiteLevelAny)]
        public IActionResult Dashboard()
        {
            return View(base.Site);
        }

        [Authorize(policy: PolicyNames.StubPolicy)]
        public IActionResult Library()
        {
            ViewData["SiteId"] = Site.Id;

            return View();
        }

        [Authorize(policy: PolicyNames.SiteNavMenusRead)]
        public IActionResult NavMenus(string navigationMenuId = null, bool create = false)
        {
            ViewData["SiteId"] = Site.Id;
            ViewData["NavigationMenuId"] = navigationMenuId;
            ViewData["Create"] = create;

            return View();
        }

        [Authorize(policy: PolicyNames.StubPolicy)]
        [RequireFeature(FeatureId.Notifications)]
        public IActionResult Notifications()
        {
            ViewData["ownerLevel"] = OwnerLevel.Site;
            ViewData["ownerId"] = Site.Id;

            return View();
        }


        [Authorize(policy: PolicyNames.SiteGroupsRead)]
        public IActionResult Groups()
        {
            var site = base.Site;

            ViewData["userGroupType"] = UserGroupType.ConnectionGroup.ToString();
            ViewData["ownerLevel"] = OwnerLevel.Site.ToString();
            ViewData["ownerId"] = site.Id;
            ViewData["poolId"] = site.SecurityPoolId;
            ViewData["userGroupTitle"] = "Connection Groups";

            return View("Groups");
        }

        [Authorize(policy: PolicyNames.StubPolicy)]
        [RequireFeature(FeatureId.Notifications)]
        public IActionResult NotifyGroups()
        {
            var site = base.Site;

            ViewData["userGroupType"] = UserGroupType.NotificationGroup.ToString();
            ViewData["ownerLevel"] = OwnerLevel.Site.ToString();
            ViewData["ownerId"] = site.Id;
            ViewData["poolId"] = site.SecurityPoolId;
            ViewData["userGroupTitle"] = "Notification Groups";

            return View("Groups");
        }

        [Authorize(policy: PolicyNames.SitePagesRead)]
        public async Task<IActionResult> Pages(string pageId = null, bool create = false)
        {
            string siteId = base.Site.Id;
            string masterPageId = null;

            if (create == false && pageId == null)
            {
                var pages = await _pageManager.GetPagesAsync(siteId);
                if (pageId == null && pages.Count > 0)
                {
                    pageId = pages.First().Id;
                    masterPageId = pages.First().MasterPage.Id;
                }
            }
            else if (pageId != null)
            {
                var page = await _pageManager.GetByIdAsync(pageId);
                if (page != null)
                    masterPageId = page.MasterPage.Id;
            }

            ViewData["SiteId"] = siteId;
            ViewData["PageId"] = pageId;
            ViewData["MasterPageId"] = masterPageId;

            return View();
        }

        [Authorize(policy: PolicyNames.SiteMasterPagesRead)]
        public IActionResult MasterPages()
        {
            ViewData["SiteId"] = Site.Id;

            return View();
        }

        [Authorize(policy: PolicyNames.SiteRolesRead)]
        public async Task<IActionResult> Roles(string roleId = null, bool create = false)
        {
            var site = base.Site;
            var poolId = site.SecurityPoolId;

            if (create == false && roleId == null)
            {
                var roles = await _poolManager.GetRolesAsync(poolId);
                roleId = roles?.FirstOrDefault()?.Id;
            }

            ViewData["ClientId"] = site.ClientId;
            ViewData["SiteId"] = site.Id;
            ViewData["PoolId"] = poolId;
            ViewData["RoleId"] = roleId;
            ViewData["ShowPools"] = false;

            return View();
        }

        [Authorize(policy: PolicyNames.SiteSettingsRead)]
        public IActionResult Settings()
        {
            ViewData["SiteId"] = Site.Id;

            return View();
        }

        [HttpGet]
        [Authorize(policy: PolicyNames.CorpUser)]
        public async Task<IActionResult> Templates(string templateId = null)
        {
            var site = base.Site;
            var productContext = await _clientManager.GetProductContextAsync(site.ClientProductAppId);

            var model = new SiteTemplateViewModel();

            model.SiteId = site.Id;
            model.AvailableTemplates = productContext.SiteTemplates;
            model.ActiveTemplate = model.AvailableTemplates.FirstOrDefault(x => x.Id == site.SiteTemplateId);

            if (templateId != null)
                model.SelectedTemplate = model.AvailableTemplates.FirstOrDefault(x => x.Id == templateId);
            else
                model.SelectedTemplate = model.ActiveTemplate;

            if (model.SelectedTemplate.Id == site.SiteTemplateId)
                model.SelectedTheme = model.SelectedTemplate.Themes.First(x => x.Id == site.ThemeId);
            else
                model.SelectedTheme = model.SelectedTemplate.Themes.First(x => x.IsDefault);

            return View(model);
        }

        [HttpPost]
        [Authorize(policy: PolicyNames.SiteTemplatesEdit)]
        public async Task<IActionResult> Templates([FromForm] string templateId, [FromForm] string themeId)
        {
            if (ModelState.IsValid)
            {
                await _siteManager.UpdateTemplateTheme(Site.Id, templateId, themeId);

                return RedirectToSiteAction("templates");
            }

            return ErrorView(ModelState);
        }

        [Authorize(policy: PolicyNames.SiteUsersRead)]
        public async Task<IActionResult> Users(string userId = null, bool create = false)
        {

            var site = base.Site;

            // TODO: Directories are mapped to sites via the sites security pool (eg, a subset). The logic below is incorrect
            var directory = await _directoryManager.GetDefaultMappedDirectoryAsync(site.SecurityPoolId);

            ViewData["SiteId"] = site.Id;
            ViewData["DirectoryId"] = directory.Id;

            return View();
        }
    }
}
