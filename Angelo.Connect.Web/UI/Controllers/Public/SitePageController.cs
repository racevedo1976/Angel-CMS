using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Data;
using Angelo.Connect.Rendering;
using Angelo.Connect.Services;
using Angelo.Connect.Models;
using Angelo.Connect.Security;
using Angelo.Connect.Security.Services;

namespace Angelo.Connect.Web.UI.Controllers.Public
{
    public class SitePageController : Controller
    {
        private PageSecurityService _pageSecurity;
        private PageManager _pageManager;
        private PageMasterManager _masterPageManager;
        private ConnectDbContext _dbContext;
        private SiteTemplateManager _templateManager;
        private SiteContext _siteContext;
        private ContentManager _contentManager;
        private IContextAccessor<SiteContext> _siteContextAccessor;
        private IContextAccessor<UserContext> _userContextAccessor;

        private string _toolbarDisplayMode = "/UI/Views/Public/SitePage/ToolbarDisplayMode.cshtml";
        private string _toolbarDesignMode = "/UI/Views/Public/SitePage/ToolbarDesignPage.cshtml";

        public SitePageController
        (
            PageSecurityService pageSecurity, 
            PageManager pageManager,
            PageMasterManager masterPageManager, 
            ContentManager widgetManager,
            SiteTemplateManager templateManager,
            IContextAccessor<SiteContext> siteContextAccessor,
            IContextAccessor<UserContext> userContextAccessor
        )
        : base()
        {
            _pageSecurity = pageSecurity;
            _pageManager = pageManager;
            _masterPageManager = masterPageManager;
            _templateManager = templateManager;
            _contentManager = widgetManager;
            _siteContextAccessor = siteContextAccessor;
            _userContextAccessor = userContextAccessor;

            _siteContext = _siteContextAccessor.GetContext();
        }

        #region Versioning Actions

        [HttpGet, Route("/sys/page/{id}/versions")]
        public async Task<IActionResult> GetVersions(string id)
        {
            var user = _userContextAccessor.GetContext();
            var page = await _pageManager.GetByIdAsync(id);

            if (!_pageSecurity.CanDesignPage(user, page))
                return this.Unauthorized();

            var versions = await _pageManager.GetVersions(id);

            ViewData["VersionDialogTitle"] = "Page Designer - Version Selector";

            ViewData["VersionPreviewRoute"] = $"/sys/page/{id}/preview";
            //ViewData["VersionPreviewRoute"] = $"{page.Path}";  <-- same as sites pages logic.
            ViewData["VersionDesignRoute"] = $"/sys/page/{id}/design";
            ViewData["VersionCreateRoute"] = $"/sys/page/{id}/versions";

            return PartialView("/UI/Views/Public/SitePage/VersionsModal.cshtml", versions);
        }

        [HttpPost, Route("/sys/page/{id}/versions")]
        public async Task<IActionResult> CreateVersion(string provider, string id, [FromForm] string label)
        {
            var user = _userContextAccessor.GetContext();
            var page = await _pageManager.GetByIdAsync(id);

            if (!_pageSecurity.CanDesignPage(user, page))
                return this.Unauthorized();

            var draft = await _pageManager.CreateDraftVersion(id, user.Name, label);

            return Redirect($"/sys/page/{id}/design?version={draft.VersionCode}");
        }

        [HttpPost, Route("/sys/page/{id}/revert")]
        public async Task<IActionResult> RevertVersion(string provider, string id, [FromQuery] string version)
        {
            var user = _userContextAccessor.GetContext();
            var page = await _pageManager.GetByIdAsync(id);

            if (!_pageSecurity.CanDesignPage(user, page))
                return this.Unauthorized();

            var pageVersions = await _pageManager.GetVersions(id);
            var thisVersion = pageVersions.FirstOrDefault(x => x.VersionCode == version);

            if (thisVersion == null)
                return BadRequest("Cannot delete the specified version. Version does not exist.");

            if (thisVersion.Status == ContentStatus.Published)
                return BadRequest("Cannot delete a published version from the designer.");

            // HOTFIX: Don't remove the only version of a page (will cause an error)
            if(pageVersions.Any(x => x.VersionCode != version))
            {
                await _pageManager.DeleteVersion(id, version);

                // redirect to the published version (if exists)
                var publishedVersion = pageVersions.FirstOrDefault(x => x.Status == ContentStatus.Published);

                if(publishedVersion != null)
                    return Redirect(page.Path);

                // otherwise redirect to the most recent version
                var latestVersion = pageVersions.OrderByDescending(x => x.VersionCode).First();
  
                return Redirect($"/sys/page/{id}?version=" + latestVersion.VersionCode);
            }

            // else redirect to site root
            return Redirect("/");                
        }

        [HttpPost, Route("/sys/page/{id}/publish")]
        public async Task<IActionResult> PublishVersion(string provider, string id, [FromQuery] string version)
        {
            var user = _userContextAccessor.GetContext();
            var page = await _pageManager.GetByIdAsync(id);

            if (page == null)
                return this.BadRequest($"Cannot publish page. Page {id} does not exist.");

            if (!_pageSecurity.CanPublishPage(user, page))
                return this.Unauthorized();
           
            await _pageManager.PublishVersion(id, version);

            return Redirect(page.Path);
        }

        #endregion

        #region Rendering Actions  
        [HttpGet /* Route defined in Startup */]
        [ResponseCache(Duration = 0)]
        public async Task<IActionResult> RenderRoute(string route, [FromQuery] string version = null)
        {
            if (!route.StartsWith("/"))
                route = "/" + route;

            // static files not resolved by the file system may also get routed here
            // return 404 since content pages should not have extensions
            if (!HasFileExt(route))
            {
                var siteId = _siteContext.SiteId;
                var page = await _pageManager.GetByRouteAsync(siteId, route);
                var user = _userContextAccessor.GetContext();

                if (page == null)
                    return NotFound();
         
                if (!_pageSecurity.CanViewPage(user, page))
                    return Unauthorized();

                var versionInfo = (version != null)
                    ? await _pageManager.GetVersion(page.Id, version)
                    : await GetPublishedOrLatestVersion(page.Id);

                
                return await RenderMaster(page, versionInfo, false, _toolbarDisplayMode);
            }

            return NotFound();
        }

        [HttpGet, Route("/sys/page/{id}")]
        public async Task<IActionResult> RenderPage(string id, [FromQuery]string version)
        {
            var page = await _pageManager.GetByIdAsync(id);
            var user = _userContextAccessor.GetContext();

            if (page == null)
                return NotFound();

            if (!_pageSecurity.CanViewPage(user, page))
                return Unauthorized();

            var versionInfo = await _pageManager.GetVersion(page.Id, version);

            return await RenderMaster(page, versionInfo, false, _toolbarDisplayMode);
        }

        [HttpGet, Route("/sys/page/{id}/preview")]
        public async Task<IActionResult> PreviewPage(string id, [FromQuery]string version)
        {
            var page = await _pageManager.GetByIdAsync(id);
            var user = _userContextAccessor.GetContext();

            if (page == null)
                return NotFound();
          
            // Note: Preview requires design priviledges since only authorized designers should have access
            // to viewing pages still under construction
            if (!_pageSecurity.CanDesignPage(user, page))
                return Unauthorized();

            var versionInfo = await _pageManager.GetVersion(page.Id, version);

            return await RenderMaster(page, versionInfo, false, _toolbarDisplayMode);
        }

        [HttpGet, Route("/sys/page/{id}/design")]
        public async Task<IActionResult> DesignPage(string id, [FromQuery]string version)
        {
            var page = await _pageManager.GetByIdAsync(id);
            var user = _userContextAccessor.GetContext();

            if (page == null)
                return NotFound();

            if (!_pageSecurity.CanDesignPage(user, page))
                return Unauthorized();

            var versionInfo = await _pageManager.GetVersion(page.Id, version);

            if (versionInfo.Status == ContentStatus.Archived)
            {
                versionInfo = await _pageManager.CreateDraftVersion(id, user.Name, null, version);
            }

            return await RenderMaster(page, versionInfo, true, _toolbarDesignMode);
        }

        #endregion

        [HttpGet, Route("/sys/oops")]
        public IActionResult oops()
        {
            return View("~/UI/Views/Public/SitePage/oops.cshtml");
        }

        #region Private

        private async Task<IActionResult> RenderMaster(Page page, IContentVersion versionInfo, bool editable, string toolbarView = null)
        {
            var contentTreeId = await _contentManager.GetContentTreeId(page.Id, versionInfo.VersionCode);

            var contentBindings = new ContentBindings
            {
                ContentId = page.Id,
                ContentType = typeof(Page).FullName,
                VersionCode = versionInfo.VersionCode,
                ViewPath = "~/UI/Views/Public/Layouts/" + (page.Layout ?? "Default") + ".cshtml",
                ViewModel = page,
                Editable = editable,
            };

            var masterPageSettings = new MasterPageSettings
            {
                MasterPageId = page.PageMasterId,
            };

            var windowSettings = new ShellSettings
            {
                WindowTitle = page.Title,
                MetaTags = BuildPageMeta(page)
            };

            if (toolbarView != null)
                windowSettings.Toolbar = new ToolbarSettings(toolbarView, page);

            return this.MasterPageView(contentBindings, masterPageSettings, windowSettings);
        }

        private async Task<IContentVersion> GetPublishedOrLatestVersion(string pageId)
        {
            var publishedVesion = await _pageManager.GetPublishedVersion(pageId);

            if (publishedVesion != null)
                return publishedVesion;

            return await _pageManager.GetLatestDraftVersion(pageId);
        }

        private Dictionary<string, string> BuildPageMeta(Page page)
        {
            var meta = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(page.Keywords))
                meta.Add("keywords", page.Keywords);

            if (!string.IsNullOrEmpty(page.Summary))
                meta.Add("description", page.Summary);

            return meta;
        }

        private bool HasFileExt(string path)
        {
            //var fileExtRegex = new Regex("\\.+\\.([^\\.]+)$");
            //var result = fileExtRegex.Match(path);
            //return result.Success;

            var ext = System.IO.Path.GetExtension(path);

            return !String.IsNullOrWhiteSpace(ext);
        }

        #endregion
    }
}
