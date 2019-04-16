using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Icons;
using Angelo.Connect.Security;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.UI;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Http;
using Angelo.Common.Mvc;
using Angelo.Connect.Extensions;
using Angelo.Connect.Models;

namespace Angelo.Connect.Menus
{
    public class UserContentMenu : IMenuItemProvider
    {
        private string _siteId;
        private string _tenant;
        private string _clientId;
        private string _corpId;

        public string MenuName { get; } = MenuType.UserContent;

        public IEnumerable<IMenuItem> MenuItems { get; set; }


        public UserContentMenu(IContextAccessor<SiteContext> siteContextAccessor, Routes routes,
            IHttpContextAccessor httpContextAccessor)
        {
            var siteContext = siteContextAccessor.GetContext();

            if (siteContext != null)
            {
                _siteId = siteContext.SiteId;
                _tenant = siteContext.TenantKey;
                _clientId = siteContext.Client.Id;
                _corpId = ConnectCoreConstants.CorporateId;

                var isSiteAlertsFeatureEnabled =
                    siteContext.ProductContext.Features.Get(FeatureId.Alerts)?.GetSettingValue<bool>("enabled") ?? false;
                var isNotificationFeatureEnabled =
                    siteContext.ProductContext.Features.Get(FeatureId.Notifications)?.GetSettingValue<bool>("enabled") ?? false;

                var httpContext = httpContextAccessor.HttpContext;
                var returnUrl = httpContext.Request.GetRelativeUrlEncoded();

                MenuItems = new List<IMenuItem>()
                {
                    // NOTE: This menu provider is part of core, thus only "known" core types of content should be listed here
                    //       So far, we have none. "Create Blog Link" moved to Blog.UserContentMenu
                    new MenuItemSecureCustom()
                    {
                        Title = "Add Notifications",
                        Url = routes.Account("notifications") + "?ru=" + returnUrl,
                        Icon = IconType.Message,
                         AuthorizeCallback = user =>
                        {
                            return user.SecurityClaims.Any(x =>
                                (((x.Type == SiteClaimTypes.SiteNotificationsSend) && ((x.Value == _siteId) || (x.Value == _clientId) || (x.Value == _corpId)) ||
                                (x.Type == SiteClaimTypes.SitePrimaryAdmin) && ((x.Value == _siteId) || (x.Value == _clientId) || (x.Value == _corpId))) ||
                                (x.Type.StartsWith("corp-") && x.Value == ConnectCoreConstants.CorporateId)) &&
                                isNotificationFeatureEnabled);
                        }
                    },

                    
                    new MenuItemSecureCustom()
                    {
                        Title = "Create Alert",
                        Url = "javascript: void $.console('alerts', '/sys/console/sitealerts/create')",
                        Icon = IconType.Alert,
                        AuthorizeCallback = user =>
                        {
                            return user.SecurityClaims.Any(x =>
                                ((x.Type == ClientClaimTypes.PrimaryAdmin) ||
                                 (x.Type == SiteClaimTypes.SitePrimaryAdmin) ||
                                 (x.Type.StartsWith("corp-") && x.Value == ConnectCoreConstants.CorporateId)) &&
                                isSiteAlertsFeatureEnabled);
                        }

                    }
                };
            }
        }

    }
}
