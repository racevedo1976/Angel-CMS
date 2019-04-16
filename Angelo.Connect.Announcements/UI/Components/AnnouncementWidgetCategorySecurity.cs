using Angelo.Connect.UI.Components;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Identity;
using Angelo.Identity.Services;
using Angelo.Connect.Security;
using System.Security.Claims;
using Angelo.Connect.Announcement.Security;
using Angelo.Connect.Announcement.Services;


namespace Angelo.Connect.Announcement.UI.Components
{
    public class AnnouncementWidgetCategorySecurity : ViewComponent //SecurityUserRoleClaims
    {
        private IContextAccessor<UserContext> _userContextAccessor;
        private IContextAccessor<SiteContext> _siteContextAccessor;
        private SecurityUserRoleClaims _securityUserRoleClaims;
        private AnnouncementManager _announcementManager;

        public AnnouncementWidgetCategorySecurity(
            AnnouncementManager announcementManager,
            SecurityUserRoleClaims securityUserRoleClaims,
            IContextAccessor<UserContext> userContextAccessor,
            IContextAccessor<SiteContext> siteContextAccessor
        ) 
        {
            _announcementManager = announcementManager;
            _securityUserRoleClaims = securityUserRoleClaims;
            _userContextAccessor = userContextAccessor;
            _siteContextAccessor = siteContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync(string categoryId = "")
        {

            IList<SecurityClaimConfig> configurations = new List<SecurityClaimConfig>();

            var userContext = _userContextAccessor.GetContext();
            var siteContext = _siteContextAccessor.GetContext();

            if (!string.IsNullOrEmpty(categoryId))
            {
                var announcementCategory = _announcementManager.GetAnnouncementCategory(categoryId);

                if (announcementCategory.UserId != userContext.UserId)
                {
                    throw new UnauthorizedAccessException();
                }
            }

            if (configurations == null)
                configurations = new List<SecurityClaimConfig>();

            SecurityClaimConfig config = new SecurityClaimConfig
            {
                Title = "Contribute / Post",
                AllowRoles = true,
                AllowUsers = true,
                AllowGroups = true,
                Claim = new Claim(AnnouncementClaimTypes.AnnouncementCategoryContribute, categoryId),
                SecurityPoolId = siteContext.SecurityPoolId,
                ResourceType = typeof(Models.AnnouncementCategory).ToString()
            };

            configurations.Add(config); 

            return await _securityUserRoleClaims.InvokeAsync(configurations); 

        }
    }
}
