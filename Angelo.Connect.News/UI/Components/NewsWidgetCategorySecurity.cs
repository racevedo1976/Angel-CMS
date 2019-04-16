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
using Angelo.Connect.News.Security;
using Angelo.Connect.News.Services;


namespace Angelo.Connect.News.UI.Components
{
    public class NewsWidgetCategorySecurity : ViewComponent //SecurityUserRoleClaims
    {
        private IContextAccessor<UserContext> _userContextAccessor;
        private IContextAccessor<SiteContext> _siteContextAccessor;
        private SecurityUserRoleClaims _securityUserRoleClaims;
        private NewsManager _NewsManager;

        public NewsWidgetCategorySecurity(
            NewsManager NewsManager,
            SecurityUserRoleClaims securityUserRoleClaims,
            IContextAccessor<UserContext> userContextAccessor,
            IContextAccessor<SiteContext> siteContextAccessor
        ) 
        {
            _NewsManager = NewsManager;
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
                var NewsCategory = _NewsManager.GetNewsCategory(categoryId);

                if (NewsCategory.UserId != userContext.UserId)
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
                Claim = new Claim(NewsClaimTypes.NewsCategoryContribute, categoryId),
                SecurityPoolId = siteContext.SecurityPoolId,
                ResourceType = typeof(Models.NewsCategory).ToString()
            };

            configurations.Add(config); 

            return await _securityUserRoleClaims.InvokeAsync(configurations); 

        }
    }
}
