using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Extensions;
using Angelo.Connect.Icons;
using Angelo.Connect.Models;
using Angelo.Connect.Security;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.UI;

namespace Angelo.Connect.Menus
{
    public class UserOptionsMenu : IMenuItemProvider
    {       
        public string MenuName { get; } = MenuType.UserOptions;

        public IEnumerable<IMenuItem> MenuItems { get; set; }


        public UserOptionsMenu(IContextAccessor<SiteContext> siteContextAccessor, Routes routes)
        {
            var siteContext = siteContextAccessor.GetContext();

            var isSiteAlertsFeatureEnabled =
                siteContext.ProductContext.Features.Get(FeatureId.Alerts)?.GetSettingValue<bool>("enabled") ?? false;
            var isNotificationFeatureEnabled =
                siteContext.ProductContext.Features.Get(FeatureId.Notifications)?.GetSettingValue<bool>("enabled") ?? false;
            // Yes - SiteContext is intended, not SiteAdminContext since user options should tailored
            // based on user capabilities within the scope of this site (or above)
            MenuItems = new List<IMenuItem>()
            {
                new MenuItemSecure() {
                    Title = "My Profile",
                    Url = routes.Account("profile"),
                    Icon = IconType.User,
                    SortOrder = 1
                },

                new MenuItemSecureCustom() {
                    Title = "My Library",
                    Url = routes.Account("library"),
                    Icon = IconType.Cloud,
                    SortOrder = 2,
                    AuthorizeCallback = user => {
                        return user.SecurityClaims.Any(x =>
                            (x.Type == UserClaimTypes.PersonalLibraryOwner) ||
                            (x.Type == ClientClaimTypes.PrimaryAdmin) ||
                            (x.Type == SiteClaimTypes.SitePrimaryAdmin) ||
                            (x.Type.StartsWith("corp-") && x.Value == ConnectCoreConstants.CorporateId)
                        );
                    }
                },

                new MenuItemSecureCustom() {
                    Title = "Notifications",
                    Url = routes.Account("notifications"),
                    Icon = IconType.Message,
                    SortOrder = 3,
                    AuthorizeCallback = user => {
                        return isNotificationFeatureEnabled;
                    }
                },

                new MenuItemSecureCustom()
                {
                    Title = "Connection Groups",
                    Url = $"javascript: void loadDialogModalByRoute('{routes.Account("groups")}','connection_groups')",
                    Icon = IconType.UserGroup,
                    SortOrder = 4,
                    AuthorizeCallback = user => {
                        return user.SecurityClaims.Any(x =>
                            (x.Type == UserClaimTypes.PersonalGroupOwner) ||
                            (x.Type == ClientClaimTypes.PrimaryAdmin) ||
                            (x.Type == SiteClaimTypes.SitePrimaryAdmin) 
                        );
                    }
                    //AuthorizedClaims = new List<SecurityClaim>()
                    //{
                    //    // eg, issued by anyone (corp, client, or any site)
                    //    new SecurityClaim(UserClaimTypes.PersonalGroupOwner, "*"),
                    //    new SecurityClaim(ClientClaimTypes.PrimaryAdmin, "*"),
                    //    new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, "*")
                    //} 
                },

                new MenuItemSecureCustom
                {
                    Title = "Corporate Admin",
                    Url = routes.CorpAdmin("dashboard"),
                    Icon = IconType.HomeAdmin,
                    SortOrder = 5,
                    AuthorizeCallback = user => {
                        return user.SecurityClaims.Any(x =>
                            x.Type.StartsWith("corp-") && x.Value == ConnectCoreConstants.CorporateId
                        );
                    }
                },

                new MenuItemSecureCustom
                {
                    Title = "Client Admin",
                    Url = routes.ClientAdmin("dashboard"),
                    Icon = IconType.SiteMap,
                    SortOrder = 6,
                    AuthorizeCallback = user => {
                        return user.SecurityClaims.Any(x =>
                            x.Type.StartsWith("client-") && (x.Value == siteContext.Client.Id || x.Value == ConnectCoreConstants.CorporateId)
                        );
                    }
                },

                new MenuItemSecureCustom
                {
                    Title = "Site Admin",
                    Url = routes.SiteAdmin("dashboard"),
                    Icon = IconType.Settings,
                    SortOrder = 7,
                    AuthorizeCallback = user => {
                        return user.SecurityClaims.Any(x =>
                            x.Type.StartsWith("site-") && (x.Value == siteContext.SiteId || x.Value == siteContext.Client.Id || x.Value == ConnectCoreConstants.CorporateId)
                        );
                    }
                },

                // Opens user console for "pages" component only
                new MenuItemSecureCustom()
                {
                    Title = "My Pages",
                    Url = "javascript: void $.console('pages')",
                    Icon = IconType.Console,
                    SortOrder = 8,
                    AuthorizeCallback = userContext => {
                        // Who this link if the user has an owner claim to any page.
                        // "My Pages" will limit access to which ones
                        return userContext.SecurityClaims.Any(x => x.Type == PageClaimTypes.PageOwner);
                    }
                },

                 // Opens user console for all components
                 // Testing: only shows for michael
                 new MenuItemSecureCustom()
                 {
                    Title = "User Console",
                    Url = "javascript: void $.console()",
                    Icon = IconType.Console,
                    SortOrder = 9,
                    AuthorizeCallback = userContext => {
                        return userContext.Name.ToLower() == "michael" || userContext.Name.ToLower() == "admin";
                    }
                 },

                // Opens user console for alerts
                new MenuItemSecureCustom()
                {
                    Title = "Manage My Alerts",
                    Url = "javascript: void $.console('alerts')",
                    Icon = IconType.Alert,
                    SortOrder = 11,
                    AuthorizeCallback = user => {
                        return user.SecurityClaims.Any(x =>
                                ((x.Type == ClientClaimTypes.PrimaryAdmin) ||
                                (x.Type == SiteClaimTypes.SitePrimaryAdmin) ||
                                (x.Type.StartsWith("corp-") && x.Value == ConnectCoreConstants.CorporateId)) &&
                                isSiteAlertsFeatureEnabled

                            //(x.Type.StartsWith("client-") && (x.Value == siteContext.Client.Id || x.Value == ConnectCoreConstants.CorporateId)) ||
                            //(x.Type.StartsWith("site-") && (x.Value == siteContext.SiteId || x.Value == siteContext.Client.Id || x.Value == ConnectCoreConstants.CorporateId)) 
                        );
                    }
                },


                new MenuItemSecure() {
                    Title = "Log Out",
                    Url = routes.Account("logout"),
                    Icon = IconType.Logout,
                    SortOrder = 99
                },

                //new MenuItemSecure() { Title = "Subscriptions", Url = "/admin/user/subscriptions", Icon = IconType.Message },
                //new MenuItemSecure() { Title = "Manage Assignments", Url = "/sys/account/assignments/userassignments", Icon = IconType.Briefcase },

            };
        }

        public bool Authorize(UserContext user)
        {
            return user.IsAuthenticated;
        }
    }
}
