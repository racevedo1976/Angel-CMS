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
    public class PagePrivateSecurity : ViewComponent
    {
        private IContextAccessor<AdminContext> _adminContextAccessor;
        private SiteContext _siteContext;
        private DirectoryManager _directoryManager;
        private SecurityUserRoleClaims _securityUserRoleClaims;

        public PagePrivateSecurity
        (
            IContextAccessor<AdminContext> adminContextAccessor, 
            SiteContext siteContext,
            DirectoryManager directoryManager, 
            SecurityUserRoleClaims securityUserRoleClaims
        ) 
        {
            _adminContextAccessor = adminContextAccessor;
            _siteContext = siteContext;
            _directoryManager = directoryManager;
            _securityUserRoleClaims = securityUserRoleClaims;
        }

        public async Task<IViewComponentResult> InvokeAsync(string pageId = "")
        {
            IList<SecurityClaimConfig> configurations = new List<SecurityClaimConfig>();
            var userContext = _adminContextAccessor.GetContext().UserContext;
            var securityPoolId = _siteContext.SecurityPoolId;
           
            if (configurations == null)
                configurations = new List<SecurityClaimConfig>();

            SecurityClaimConfig config = new SecurityClaimConfig
            {
                Title = "Private Audience",
                Description = "Manage who can view this page.",
                AllowRoles = true,
                AllowUsers = true,
                Claim = new Claim(PageClaimTypes.ViewPrivatePage, pageId),
                SecurityPoolId = securityPoolId, 
                ResourceType = typeof(Models.Page).ToString()
            };

            configurations.Add(config);

            var componentId = (string)ViewData["cid"];

            return await _securityUserRoleClaims.InvokeAsync(configurations, componentId);
        }
    }
}
