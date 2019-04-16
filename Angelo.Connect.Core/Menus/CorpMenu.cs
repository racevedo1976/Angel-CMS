using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Icons;
using Angelo.Connect.Security;
using Angelo.Connect.Security.KnownClaims;

namespace Angelo.Connect.Menus
{
    public class CorpMenu : IMenuItemProvider
    {
        private string _corpId = ConnectCoreConstants.CorporateId;

        public string MenuName { get; } = MenuType.CorporateTools;

        public IEnumerable<IMenuItem> MenuItems { get; set; }
        public string SidePanelView { get; set; }

        public CorpMenu()
        {
            BuildMenuItems();
        }

        private void BuildMenuItems() { 
            MenuItems = new List<IMenuItem>()
            {
                new MenuItemSecureClaims() {
                    Title = "Manage Clients",
                    Url = "/sys/corp/admin/clients",
                    Icon = IconType.Briefcase, Active = true,

                    AuthorizedClaims = new List<SecurityClaim>
                    {
                        new SecurityClaim(CorpClaimTypes.CorpCustomersRead, _corpId),
                        new SecurityClaim(CorpClaimTypes.CorpPrimaryAdmin, _corpId),
                    }
                },

                new MenuItemSecureClaims() {
                    Title = "Manage Sites",
                    Url = "/sys/corp/admin/sites",
                    Icon = IconType.Sites,

                    AuthorizedClaims = new List<SecurityClaim>
                    {
                        // TODO: Add corp level claims for corp level site setup
                        new SecurityClaim(CorpClaimTypes.CorpPrimaryAdmin, _corpId),
                    }
                },
            
                new MenuItemSecureClaims()
                {
                    Title = "Job Console",
                    Url = "/sys/corp/admin/jobs",
                    Icon = IconType.TaskList,

                    AuthorizedClaims = new List<SecurityClaim>
                    {
                        new SecurityClaim(CorpClaimTypes.CorpJobsRead, _corpId),
                        new SecurityClaim(CorpClaimTypes.CorpJobsExecute, _corpId),
                        new SecurityClaim(CorpClaimTypes.CorpPrimaryAdmin, _corpId),
                    }
                },

                new MenuItemSecureClaims()
                {
                    Title = "OpenId Meta",
                    Url = "/sys/corp/admin/aegis",
                    Icon = IconType.Fire,

                    AuthorizedClaims = new List<SecurityClaim>
                    {
                        new SecurityClaim(CorpClaimTypes.CorpPrimaryAdmin, _corpId)
                    }
                },
                

                new MenuItemSecureClaims()
                {
                    Title = "Ldap Settings",
                    Url = "/sys/corp/admin/LdapSettings",
                    Icon = IconType.Ldap,

                    AuthorizedClaims = new List<SecurityClaim>
                    {
                        new SecurityClaim(CorpClaimTypes.CorpPrimaryAdmin, _corpId)
                    }
                },
            };
        }        
    }
}
