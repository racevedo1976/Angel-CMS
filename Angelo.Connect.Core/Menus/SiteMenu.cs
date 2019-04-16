using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Extensions;
using Angelo.Connect.Icons;
using Angelo.Connect.Security;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Models;


namespace Angelo.Connect.Menus
{
    public class SiteMenu : IMenuItemProvider
    {
        private string _siteId;
        private string _tenant;
        private string _clientId;
        private string _corpId;

        public string MenuName { get; } = MenuType.SiteTools;

        public IEnumerable<IMenuItem> MenuItems { get; set; }
        
        public SiteMenu(IContextAccessor<AdminContext> adminContextAccessor)
        {
            var context = adminContextAccessor.GetContext();
            var site = context.SiteContext?.Site;

            if (site != null)
            {
                _siteId = site.Id;
                _tenant = site.TenantKey;
                _clientId = site.ClientId;
                _corpId = context.CorpId;

                BuildMenuItems(context);
            }
        }
    
        private void BuildMenuItems(AdminContext context) {
            var siteId = _siteId;
            var isNotificationsEnabled = context.SiteContext.Product.Features.Get(FeatureId.Notifications)?.GetSettingValue<bool>("enabled") ?? false;

            MenuItems = new List<IMenuItem>()
            {
                new MenuItemSecureClaims() {
                    Title = "User Accounts",
                    Url =  $"/sys/sites/{_tenant}/admin/users",
                    Icon = IconType.User,
                    AuthorizedClaims = new List<SecurityClaim>
                    {
                        new SecurityClaim(SiteClaimTypes.SiteUsersRead, _siteId),
                        new SecurityClaim(SiteClaimTypes.SiteUsersRead, _clientId),
                        new SecurityClaim(SiteClaimTypes.SiteUsersRead, _corpId),

                        new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, _siteId),
                        new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, _clientId),
                        new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, _corpId),
                    }
                },

                new MenuItemSecureClaims() {
                    Title = "Security Roles",
                    Url = $"/sys/sites/{_tenant}/admin/roles",
                    Icon = IconType.Roles,
                    AuthorizedClaims = new List<SecurityClaim>
                    {
                        new SecurityClaim(SiteClaimTypes.SiteRolesRead, _siteId),
                        new SecurityClaim(SiteClaimTypes.SiteRolesRead, _clientId),
                        new SecurityClaim(SiteClaimTypes.SiteRolesRead, _corpId),

                        new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, _siteId),
                        new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, _clientId),
                        new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, _corpId),
                    },
                },

                new MenuItemSecureClaims() {
                    Title = "Site Settings",
                    Url = $"/sys/sites/{_tenant}/admin/settings",
                    Icon = IconType.Wrench,
                    AuthorizedClaims = new List<SecurityClaim>
                    {
                         // Site Level
                        new SecurityClaim(SiteClaimTypes.SiteSettingsRead, _siteId),
                        new SecurityClaim(SiteClaimTypes.SiteSettingsRead, _clientId),
                        new SecurityClaim(SiteClaimTypes.SiteSettingsRead, _corpId),

                        new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, _siteId),
                        new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, _clientId),
                        new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, _corpId),
                    },
                    MenuItems = new List<IMenuItem>()
                    {
                        new MenuItemSecureClaims {
                            Title = "Site Templates",
                            Url = $"/sys/sites/{_tenant}/admin/templates",
                            Icon = IconType.Theme,

                            AuthorizedClaims = new List<SecurityClaim>
                            {
                                new SecurityClaim(CorpClaimTypes.CorpUser, _corpId)

                                /*
                                new SecurityClaim(SiteClaimTypes.SiteTemplateRead, _siteId),
                                new SecurityClaim(SiteClaimTypes.SiteTemplateRead, _clientId),
                                new SecurityClaim(SiteClaimTypes.SiteTemplateRead, _corpId),

                                new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, _siteId),
                                new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, _clientId),
                                new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, _corpId),
                                */
                            },
                        },

                    }
                },

                new MenuItemSecureClaims() {
                    Title = "Navigation Menus",
                    Url = $"/sys/sites/{_tenant}/admin/navmenus",
                    Icon = IconType.Pencil,

                    AuthorizedClaims = new List<SecurityClaim>
                    {
                        // TODO: No security claims exist for navigation menus

                        new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, _siteId),
                        new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, _clientId),
                        new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, _corpId),
                    }
                },

                new MenuItemSecureClaims() {
                    Title = "Site Pages",
                    Url = $"/sys/sites/{_tenant}/admin/pages",
                    Icon = IconType.Sites,

                    AuthorizedClaims = new List<SecurityClaim>
                    {
                        new SecurityClaim(SiteClaimTypes.SitePagesRead, _siteId),
                        new SecurityClaim(SiteClaimTypes.SitePagesRead, _clientId),
                        new SecurityClaim(SiteClaimTypes.SitePagesRead, _corpId),

                        new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, _siteId),
                        new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, _clientId),
                        new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, _corpId),
                    }
                },

                 new MenuItemSecureCustom() {
                    Title = "Notifications",
                    Url = $"/sys/sites/{_tenant}/admin/notifications",
                    Icon = IconType.Message,
                    AuthorizeCallback = user =>
                    {
                        return user.SecurityClaims.Any(x =>
                            (((x.Type == SiteClaimTypes.SiteNotificationsSend) && ((x.Value == _siteId) || (x.Value == _clientId) || (x.Value == _corpId)) ||
                            (x.Type == SiteClaimTypes.SitePrimaryAdmin) && ((x.Value == _siteId) || (x.Value == _clientId) || (x.Value == _corpId))) ||
                            (x.Type.StartsWith("corp-") && x.Value == ConnectCoreConstants.CorporateId)) &&
                            isNotificationsEnabled);
                    }
                },

                 new MenuItemSecureCustom() {
                    Title = "Notification Groups",
                    Url =  $"/sys/sites/{_tenant}/admin/notifygroups",
                    Icon = IconType.UserGroup,
                    AuthorizeCallback = user =>
                    {
                        return user.SecurityClaims.Any(x =>
                            (((x.Type == SiteClaimTypes.SiteNotifyGroupsRead) && ((x.Value == _siteId) || (x.Value == _clientId) || (x.Value == _corpId)) ||
                            (x.Type == SiteClaimTypes.SitePrimaryAdmin) && ((x.Value == _siteId) || (x.Value == _clientId) || (x.Value == _corpId))) ||
                            (x.Type.StartsWith("corp-") && x.Value == ConnectCoreConstants.CorporateId)) &&
                            isNotificationsEnabled);
                    }
                },
            };
        }        
    }
}
