using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Rendering;
using Angelo.Connect.Models;
using Angelo.Connect.Services;
using Angelo.Connect.Security;
using Angelo.Connect.Security.Services;

namespace Angelo.Connect.Web.UI.Controllers.Public
{
    public class SiteMasterPageController : Controller
    {
        private PageSecurityService _pageSecurity;
        private PageMasterManager _masterPageManager;
        private SiteTemplateManager _templateManager;
        private SiteContext _siteContext;
        private ContentManager _contentManager;
        private IContextAccessor<UserContext> _userContextAccessor;

        private string _toolbarDesignMode = "/UI/Views/Public/SitePage/ToolbarDesignMaster.cshtml";

        public SiteMasterPageController
        (
            PageSecurityService pageSecurity,
            PageMasterManager masterPageManager, 
            ContentManager widgetManager,
            SiteTemplateManager templateManager,
            SiteContext siteContext,
            IContextAccessor<UserContext> userContextAccessor
        )
        : base()
        {
            _pageSecurity = pageSecurity;
            _masterPageManager = masterPageManager;
            _templateManager = templateManager;
            _siteContext = siteContext;
            _contentManager = widgetManager;
            _userContextAccessor = userContextAccessor;
        }

        #region Versioning Actions

        [HttpGet, Route("/sys/master/{id}/versions")]
        public async Task<IActionResult> GetVersions(string id)
        {            
            var user = _userContextAccessor.GetContext();
            var masterPage = await _masterPageManager.GetByIdAsync(id);

            if (!_pageSecurity.CanDesignMaster(user, masterPage))
                return this.Unauthorized();

            var versions = await _masterPageManager.GetVersions(id);

            ViewData["VersionDialogTitle"] = "Master Page Designer - Version Selector";

            ViewData["VersionPreviewRoute"] = $"/sys/master/{id}/preview";
            ViewData["VersionDesignRoute"] = $"/sys/master/{id}/design";
            ViewData["VersionCreateRoute"] = $"/sys/master/{id}/versions";


            return PartialView("/UI/Views/Public/SitePage/VersionsModal.cshtml", versions);

        }

        [HttpPost, Route("/sys/master/{id}/versions")]
        public async Task<IActionResult> CreateVersion(string id, [FromForm] string label)
        { 
            var user = _userContextAccessor.GetContext();
            var masterPage = await _masterPageManager.GetByIdAsync(id);

            if (!_pageSecurity.CanDesignMaster(user, masterPage))
                return this.Unauthorized();


            var draft = await _masterPageManager.CreateDraftVersion(id, user.Name, label);

            return Redirect($"/sys/master/{id}/design?version={draft.VersionCode}");
        }

   
        [HttpPost, Route("/sys/master/{id}/revert")]
        public async Task<IActionResult> RevertVersion(string id, [FromQuery] string version)
        {
            var user = _userContextAccessor.GetContext();
            var masterPage = await _masterPageManager.GetByIdAsync(id);

            if (!_pageSecurity.CanDesignMaster(user, masterPage))
                return this.Unauthorized();

            var publishedVersion = await _masterPageManager.GetPublishedVersion(id);

            if (publishedVersion?.VersionCode == version)
                return BadRequest("Cannot delete a published version from the designer");

            // else - delete
            await _masterPageManager.DeleteVersion(id, version);

            if (Request.Cookies.ContainsKey("ReturnUrl"))
            {
                var route = Request.Cookies["ReturnUrl"].ToString();
                return Redirect(route);
            }

            return Redirect("/");
        }


        [HttpPost, Route("/sys/master/{id}/publish")]
        public async Task<IActionResult> PublishVersion(string id, [FromQuery] string version)
        {
            var user = _userContextAccessor.GetContext();
            var masterPage = await _masterPageManager.GetByIdAsync(id);

            if (masterPage == null)
                return this.BadRequest($"Cannot publish page. Page {id} does not exist.");

            if (!_pageSecurity.CanPublishMaster(user, masterPage))
                return this.Unauthorized();

            await _masterPageManager.PublishVersion(id, version);

            if (Request.Cookies.ContainsKey("ReturnUrl"))
            {
                var route = Request.Cookies["ReturnUrl"].ToString();
                return Redirect(route);
            }
           
            return Redirect("/");
        }

        #endregion

        #region Design Actions
        [HttpGet, Route("/sys/master/{id}/preview")]      
        public async Task<IActionResult> Preview(string id, [FromQuery]string version = null)
        {
            var masterPage = await _masterPageManager.GetByIdAsync(id);
            var user = _userContextAccessor.GetContext();

            if (masterPage == null)
                return NotFound();

            // Only those who can design the master page should be able to preview changes
            if (!_pageSecurity.CanDesignMaster(user, masterPage))
                return Unauthorized();

            var versionInfo = await _masterPageManager.GetVersion(masterPage.Id, version);

            return RenderMaster(masterPage, versionInfo, false, null);
        }

        [HttpGet, Route("/sys/master/{id}/design")]
        public async Task<IActionResult> Design(string id, [FromQuery]string version = null)
        {
            var masterPage = await _masterPageManager.GetByIdAsync(id);
            var user = _userContextAccessor.GetContext();

            if (masterPage == null)
                return NotFound();

            if (!_pageSecurity.CanDesignMaster(user, masterPage))
                return Unauthorized();

            var versionInfo = await _masterPageManager.GetVersion(masterPage.Id, version);

            return RenderMaster(masterPage, versionInfo, true, _toolbarDesignMode);
        }

        #endregion


        private IActionResult RenderMaster(PageMaster masterPage, IContentVersion versionInfo, bool editable, string toolbarView = null)
        {
            var masterPageSettings = new MasterPageSettings
            {
                MasterPageId = masterPage.Id,
                VersionCode = versionInfo.VersionCode,
                Editable = editable
            };

            var contentBindings = ContentBindings.Generic("~/UI/Views/Public/Layouts/Empty.cshtml", null);
            var windowSettings = new ShellSettings(masterPage.Title);

            if (toolbarView != null)
                windowSettings.Toolbar = new ToolbarSettings(toolbarView, masterPage);

            return this.MasterPageView(contentBindings, masterPageSettings, windowSettings);
        }
      
    }
}
