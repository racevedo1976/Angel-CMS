using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Extensions;
using Angelo.Connect.Models;
using Angelo.Connect.Icons;
using Angelo.Connect.Security;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Services;

namespace Angelo.Connect.Menus
{
    public class ClientMenu : IMenuItemProvider
    {
        public string MenuName { get; } = MenuType.ClientTools;

        public IEnumerable<IMenuItem> MenuItems { get; set; }


        private string _tenant;
        private string _clientId;
        private string _corpId;

        public ClientMenu(IContextAccessor<AdminContext> adminContextAccessor, ClientManager clientManager)
        {
            var context = adminContextAccessor.GetContext();
            var client = context.ClientContext?.Client;

            if (client != null)
            {
                _corpId = context.CorpId;
                _clientId = client.Id;
                _tenant = client.TenantKey;              

                BuildMenuItems(context);
            }
        }

        private void BuildMenuItems(AdminContext context)
        {
            var isNotificationsEnabled = context.SiteContext.Product.Features.Get(FeatureId.Notifications)?.GetSettingValue<bool>("enabled") ?? false;


            MenuItems = new List<IMenuItem>()
            {

                new MenuItemSecureClaims() {
                    Title = "User Accounts",
                    Url = $"/sys/clients/{_tenant}/admin/users",
                    Icon = IconType.User,

                    AuthorizedClaims = new List<SecurityClaim>
                    {
                        new SecurityClaim(ClientClaimTypes.UsersRead, _clientId),
                        new SecurityClaim(ClientClaimTypes.UsersRead, _corpId),

                        new SecurityClaim(ClientClaimTypes.PrimaryAdmin, _clientId),
                        new SecurityClaim(ClientClaimTypes.PrimaryAdmin, _corpId),
                    },
                },

                new MenuItemSecureClaims() {
                    Title = "Security Roles",
                    Url = $"/sys/clients/{_tenant}/admin/roles",
                    Icon = IconType.Roles,

                    AuthorizedClaims = new List<SecurityClaim>
                    {
                        new SecurityClaim(ClientClaimTypes.AppRolesRead, _clientId),
                        new SecurityClaim(ClientClaimTypes.AppRolesRead, _corpId),

                        new SecurityClaim(ClientClaimTypes.PrimaryAdmin, _clientId),
                        new SecurityClaim(ClientClaimTypes.PrimaryAdmin, _corpId),
                    },
                },

                new MenuItemSecureClaims() {
                    Title = "Web Sites",
                    Url = $"/sys/clients/{_tenant}/admin/sites",
                    Icon = IconType.Sites, 

                    AuthorizedClaims = new List<SecurityClaim>
                    {
                        new SecurityClaim(ClientClaimTypes.SitesRead, _clientId),
                        new SecurityClaim(ClientClaimTypes.SitesRead, _corpId),

                        new SecurityClaim(ClientClaimTypes.PrimaryAdmin, _clientId),
                        new SecurityClaim(ClientClaimTypes.PrimaryAdmin, _corpId),
                    },
                },

                new MenuItemSecureCustom() {
                    Title = "Notifications",
                    Url = $"/sys/clients/{_tenant}/admin/notifications",
                    Icon = IconType.Message,

                    AuthorizeCallback = user =>
                    {
                        return user.SecurityClaims.Any(x =>
                            (((x.Type == ClientClaimTypes.PrimaryAdmin) && ((x.Value == _clientId) || (x.Value == _corpId)) ||
                            (x.Type == ClientClaimTypes.AppNotificationsSend) && ((x.Value == _clientId) || (x.Value == _corpId))) ||
                            (x.Type.StartsWith("corp-") && x.Value == ConnectCoreConstants.CorporateId)) &&
                            isNotificationsEnabled);
                    }
                },

                new MenuItemSecureCustom() {
                    Title = "Notification Groups",
                    Url = $"/sys/clients/{_tenant}/admin/notifygroups",
                    Icon = IconType.UserGroup,

                    AuthorizeCallback = user =>
                    {
                        return user.SecurityClaims.Any(x =>
                            (((x.Type == ClientClaimTypes.PrimaryAdmin) && ((x.Value == _clientId) || (x.Value == _corpId)) ||
                            (x.Type == ClientClaimTypes.AppNotifyGroupsRead) && ((x.Value == _clientId) || (x.Value == _corpId))) ||
                            (x.Type.StartsWith("corp-") && x.Value == ConnectCoreConstants.CorporateId)) &&
                            isNotificationsEnabled);
                    }
                }
            };
        }
    }
}
