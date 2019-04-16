using Angelo.Connect.UI.Components;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Identity;
using Angelo.Identity.Models;
using Angelo.Identity.Services;
using Angelo.Connect.Security;
using Angelo.Connect.Security.KnownClaims;
using System.Security.Claims;

namespace Angelo.Connect.Web.UI.Components
{
    public class PageSecurityForm : ViewComponent
    {
        private IContextAccessor<AdminContext> _adminContextAccessor;
        private SiteContext _siteContext;
        private DirectoryManager _directoryManager;
        private SecurityUserRoleClaims _securityUserRoleClaims;

        public PageSecurityForm
        (
            IContextAccessor<AdminContext> adminContextAccessor,
            SiteContext siteContext,
            DirectoryManager directoryManager,
            ResourceManager resourceManager,
            SecurityUserRoleClaims securityUserRoleClaims)
        {
            _adminContextAccessor = adminContextAccessor;
            _siteContext = siteContext;
            _directoryManager = directoryManager;
            _securityUserRoleClaims = securityUserRoleClaims;
        }

        public async Task<IViewComponentResult> InvokeAsync(string pageId)
        {          
            var userContext = _adminContextAccessor.GetContext().UserContext;
            var securityPoolId = _siteContext.SecurityPoolId;            
            var config = new List<SecurityClaimConfig>();


            config.Add(new SecurityClaimConfig
            {
                Title = "Page Designers",
                Description = "Manage who can design new versions of this page.",
                AllowRoles = true,
                AllowUsers = true,
                Claim = new Claim(PageClaimTypes.DesignPage, pageId),
                SecurityPoolId = securityPoolId,
                ResourceType = typeof(Models.Page).ToString()
            });

            config.Add(new SecurityClaimConfig
            {
                Title = "Section Owner",
                Description = "Manage who can create child pages under this page.",
                AllowRoles = true,
                AllowUsers = true,
                Claim = new Claim(PageClaimTypes.PageOwner, pageId),
                SecurityPoolId = securityPoolId,
                ResourceType = typeof(Models.Page).ToString()
            });

            config.Add(new SecurityClaimConfig
            {
                Title = "Page Publishers",
                Description = "Manage who can publish modified versions of this page.",
                AllowRoles = true,
                AllowUsers = true,
                Claim = new Claim(PageClaimTypes.PublishPage, pageId),
                SecurityPoolId = securityPoolId,
                ResourceType = typeof(Models.Page).ToString()
            });

            var componentId = (string)ViewData["cid"];

            return await _securityUserRoleClaims.InvokeAsync(config, componentId);
        }
    }
}
