using Angelo.Connect.Abstractions;
using Angelo.Connect.Calendar.Security;
using Angelo.Connect.Calendar.Services;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security;
using Angelo.Connect.UI.Components;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Angelo.Connect.Calendar.Components
{
   
    public class CalendarEventGroupSecurity : ViewComponent 
    {
        private IContextAccessor<UserContext> _userContextAccessor;
        private IContextAccessor<SiteContext> _siteContextAccessor;

        private SecurityUserRoleClaims _securityUserRoleClaims;
        private CalendarQueryService _calendarqueryService;

        public CalendarEventGroupSecurity(IContextAccessor<UserContext> userContextAccessor, IContextAccessor<SiteContext> siteContextAccessor, SecurityUserRoleClaims securityUserRoleClaims, CalendarQueryService calendarqueryService)
        {
            _securityUserRoleClaims = securityUserRoleClaims;
            _userContextAccessor = userContextAccessor;
            _siteContextAccessor = siteContextAccessor;
            _calendarqueryService = calendarqueryService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string eventGroupId = "")
        {

            IList<SecurityClaimConfig> configurations = new List<SecurityClaimConfig>();

            var userContext = _userContextAccessor.GetContext();
            var siteContext = _siteContextAccessor.GetContext();

            if (!string.IsNullOrEmpty(eventGroupId))
            {
                var eventGroup = _calendarqueryService.GetEventGroup(eventGroupId);

                if (eventGroup.UserId != userContext.UserId)
                {
                    throw new UnauthorizedAccessException();
                }
            }

            if (configurations == null)
                configurations = new List<SecurityClaimConfig>();

            SecurityClaimConfig config = new SecurityClaimConfig
            {
                Title = "Access to Events",
                AllowRoles = true,
                AllowUsers = true,
                AllowGroups = true,
                Claim = new Claim(CalendarClaimTypes.CalendarEventGroupContribute, eventGroupId),
                SecurityPoolId = siteContext.SecurityPoolId,
                ResourceType = typeof(CalendarEventGroups).ToString()
            };

            configurations.Add(config);

            return await _securityUserRoleClaims.InvokeAsync(configurations);

        }
    }

}





//using Angelo.Connect.UI.Components;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Angelo.Connect.Abstractions;
//using Angelo.Connect.Configuration;
//using Angelo.Identity;
//using Angelo.Identity.Services;
//using Angelo.Connect.Security;
//using System.Security.Claims;
//using Angelo.Connect.Blog.Security;
//using Angelo.Connect.Blog.Services;


//namespace Angelo.Connect.Blog.UI.Components
//{
//    public class BlogWidgetCategorySecurity : ViewComponent //SecurityUserRoleClaims
//    {
//        private IContextAccessor<AdminContext> _adminContextAccessor;
//        private SecurityUserRoleClaims _securityUserRoleClaims;
//        private BlogManager _blogManager;

//        public BlogWidgetCategorySecurity(
//            BlogManager blogManager,
//            SecurityUserRoleClaims securityUserRoleClaims,
//            IContextAccessor<AdminContext> adminContextAccessor
//        )
//        {
//            _blogManager = blogManager;
//            _securityUserRoleClaims = securityUserRoleClaims;
//            _adminContextAccessor = adminContextAccessor;
//        }

//        public async Task<IViewComponentResult> InvokeAsync(string categoryId = "")
//        {

//            IList<SecurityClaimConfig> configurations = new List<SecurityClaimConfig>();

//            var userContext = _adminContextAccessor.GetContext().UserContext;
//            if (!string.IsNullOrEmpty(categoryId))
//            {
//                var blogCategory = _blogManager.GetBlogCategory(categoryId);

//                if (blogCategory.UserId != userContext.UserId)
//                {
//                    throw new UnauthorizedAccessException();
//                }
//            }

//            if (configurations == null)
//                configurations = new List<SecurityClaimConfig>();

//            SecurityClaimConfig config = new SecurityClaimConfig
//            {
//                Title = "Contribute / Post",
//                AllowRoles = true,
//                AllowUsers = true,
//                Claim = new Claim(BlogClaimTypes.BlogCategoryContribute, categoryId),
//                UserDirectoryId = "MyCompany-directory",
//                RolePoolId = "MyCompany-client-pool",
//                ResourceType = typeof(Models.BlogCategory).ToString()
//            };

//            configurations.Add(config);

//            return await _securityUserRoleClaims.InvokeAsync(configurations);

//        }
//    }
//}
