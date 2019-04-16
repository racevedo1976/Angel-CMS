using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security;
using Angelo.Identity;
using Angelo.Identity.Models;
using Angelo.Identity.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Angelo.Connect.Models;
using Angelo.Connect.Services;

namespace Angelo.Connect.UI.Components
{
    public class SecurityUserRoleClaims : ViewComponent
    {
        private IContextAccessor<AdminContext> _adminContextAccessor;
        private DirectoryManager _directoryManager;
        private SecurityPoolManager _poolManager;
        private UserManager _userManager;
        private RoleManager _roleManager;
        private IEnumerable<ISecurityGroupProvider> _groupsProvider;
        private GroupManager _groupManager;

        //public abstract IList<SecurityClaimConfig> configurations { get; set; }

        public SecurityUserRoleClaims(
            IContextAccessor<AdminContext> adminContextAccessor,
            DirectoryManager directoryManager,
            SecurityPoolManager poolManager,
            RoleManager roleManager,
            UserManager userManager,
            IEnumerable<ISecurityGroupProvider> groupsProvider,
            GroupManager groupManager
            )
        {
            _adminContextAccessor = adminContextAccessor;
            _directoryManager = directoryManager;
            _poolManager = poolManager;
            _roleManager = roleManager;
            _userManager = userManager;
            _groupsProvider = groupsProvider;
            _groupManager = groupManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(IList<SecurityClaimConfig> configurations, string componentId = null)
        {
            IEnumerable<User> users = new List<User>();
            IEnumerable<Role> roles = new List<Role>();
            IList<SecurityClaimConfigViewModel> securityViewModel = new List<SecurityClaimConfigViewModel>();
            string defaultId = "resourceSecurity_" + Guid.NewGuid().ToString("N").Substring(0, 10);

            foreach (SecurityClaimConfig config in configurations)
            {
                SecurityClaimConfigViewModel configModel = new SecurityClaimConfigViewModel();

                //get user in directory
                if (config.AllowUsers)
                {

                    var directory = await _directoryManager.GetDefaultMappedDirectoryAsync(config.SecurityPoolId);                

                    configModel.Users = await _directoryManager.GetUsersAsync(directory.Id);

                    if (!string.IsNullOrEmpty(config.AllowUsersLabel))
                        configModel.UsersLabel = config.AllowUsersLabel;

                    configModel.SelectedUsers = (await _userManager.GetClaimObjectsAsync(config.Claim)).Select(x => x.UserId).ToList();
                    configModel.AllowUsers = config.AllowUsers;
                }

                //get roles in pool
                if (config.AllowRoles)
                {
                    configModel.Roles = await _poolManager.GetRolesAsync(config.SecurityPoolId);

                    if (!string.IsNullOrEmpty(config.AllowRolesLabel))
                        configModel.RolesLabel = config.AllowRolesLabel;

                    configModel.SelectedRoles = _roleManager.QueryRoleClaims().Where(x => x.ClaimType == config.Claim.Type && x.ClaimValue == config.Claim.Value).ToList();
                    configModel.AllowRoles = config.AllowRoles;
                }

                //groups
                if (config.AllowGroups)
                {
                    if (!string.IsNullOrEmpty(config.AllowGroupsLabel))
                        configModel.GroupsLabel = config.AllowGroupsLabel;

                    configModel.Groups = await _groupManager.GetGroupsOwnedByUser(_adminContextAccessor.GetContext().UserContext.UserId);

                    configModel.SelectedGroups = (await _groupManager.GetGroupClaimsAsync(config.Claim)).Select(x => x.GroupId).ToList();
                    configModel.AllowGroups = config.AllowGroups;
                }
                

                configModel.ResourceType = config.ResourceType;
                configModel.Title = config.Title;
                configModel.Description = config.Description;
                configModel.Claim = config.Claim;

                securityViewModel.Add(configModel);
            }

            ViewData["ComponentId"] = componentId ?? defaultId;

            return View("~/UI/Views/Components/SecurityUserRoleClaims/UserRoleClaimsView.cshtml", securityViewModel);

        }
    }
}
