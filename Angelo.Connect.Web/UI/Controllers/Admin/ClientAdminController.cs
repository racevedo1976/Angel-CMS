using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

using Angelo.Connect.Configuration;
using Angelo.Connect.Security;
using Angelo.Connect.Services;
using Angelo.Identity;
using Angelo.Identity.Services;
using Angelo.Connect.Models;

namespace Angelo.Connect.Web.UI.Controllers.Admin
{
    public class ClientAdminController : ClientControllerBase
    {
        private SecurityPoolManager _poolManager;
        private DirectoryManager _directoryManager;
        private UserManager _userManager;
        private ClientManager _clientManager;

        public ClientAdminController
        (
            UserManager userManager,
            DirectoryManager directoryManager,
            SecurityPoolManager poolManager,
            ClientManager clientManager,
            ClientAdminContextAccessor clientContextAccessor,
            IAuthorizationService authorizationService,
            ILogger<ClientAdminController> logger
        ) 
        : base(clientContextAccessor, authorizationService, logger)
        {
            _clientManager = clientManager;
            _userManager = userManager;
            _directoryManager = directoryManager;
            _poolManager = poolManager;
        }

        [Authorize(policy: PolicyNames.ClientLevelAny)]
        public async Task<IActionResult> Dashboard(string appId = null)
        {
            var client = base.Client;

            // TODO: Move application conxtext initialization to middleware once routing is refactored

            return View(client);
        }

        [Authorize(policy: PolicyNames.StubPolicy)]
        [RequireFeature(FeatureId.Notifications)]
        public IActionResult Notifications()
        {
            ViewData["ownerLevel"] = OwnerLevel.Client;
            ViewData["ownerId"] = Client.Id;

            return View();
        }

        [HttpGet]
        [Authorize(policy: PolicyNames.ClientRolesRead)]     
        public async Task<IActionResult> Roles(string poolId = null, string roleId = null, bool create = false)
        {
            var client = base.Client;
            
            if (poolId == null)
            {
                poolId = client.SecurityPoolId;
            }

            if (create == false && roleId == null)
            {
                var roles = await _poolManager.GetRolesAsync(poolId);
                roleId = roles?.FirstOrDefault()?.Id;
            }

            ViewData["ClientId"] = client.Id;
            ViewData["SiteId"] = "";
            ViewData["PoolId"] = poolId;
            ViewData["RoleId"] = roleId;
            ViewData["ShowPools"] = true;

            return View();
        }

        [Authorize(policy: PolicyNames.ClientGroupsRead)]
        public IActionResult Groups()
        {
            var client = base.Client;

            ViewData["userGroupType"] = UserGroupType.ConnectionGroup.ToString();
            ViewData["ownerLevel"] = OwnerLevel.Client.ToString();
            ViewData["ownerId"] = client.Id;
            ViewData["poolId"] = base.Client.SecurityPoolId;
            ViewData["userGroupTitle"] = "Connection Groups";
           
            return View("Groups");
        }

        [Authorize(policy: PolicyNames.ClientNotificationGroupRead)]
        [RequireFeature(FeatureId.Notifications)]
        public IActionResult NotifyGroups()
        {
            var client = base.Client;

            ViewData["userGroupType"] = UserGroupType.NotificationGroup.ToString();
            ViewData["ownerLevel"] = OwnerLevel.Client.ToString();
            ViewData["ownerId"] = client.Id;
            ViewData["poolId"] = base.Client.SecurityPoolId;
            ViewData["userGroupTitle"] = "Notification Groups";

            return View("Groups");
        }

        [Authorize(policy: PolicyNames.StubPolicy)]
        public IActionResult Library(string tenant)
        {
            ViewData["ClientId"] = Client.Id;

            return View();
        }

        [Authorize(policy: PolicyNames.StubPolicy)]
        public IActionResult Collections(string tenant, string collectionId = null)
        {
            ViewData["ClientId"] = Client.Id;
            ViewData["CollectionId"] = collectionId;
           
            return View();
        }

        [Authorize(policy: PolicyNames.ClientSitesRead)]
        public IActionResult Sites(string tenant)
        {
            ViewData["ClientId"] = Client.Id;
            ViewData["AppId"] = App.AppId;

            return View();
        }

        [Authorize(policy: PolicyNames.ClientUsersRead)]
        public async Task<IActionResult> Users(string directoryId = null, string userId = null, bool create = false)
        {
            var clientId = base.Client.Id;
            
            if (directoryId == null)
            {
                var directory = await _directoryManager.GetDefaultMappedDirectoryAsync(base.Client.SecurityPoolId);
                directoryId = directory.Id;
            }
          
            ViewData["ClientId"] = clientId;
            ViewData["DirectoryId"] = directoryId;

            return View();
        }

        [Authorize(policy: PolicyNames.ClientDirectoriesRead)]
        public async Task<IActionResult> Directories(string directoryId)
        {
            var clientId = base.Client.Id;
            var directories = await _directoryManager.GetDirectoriesAsync(Client.TenantKey);

            var model = directories.Select(x => new SelectListItem
            {
                Value = x.Id,
                Text = x.Name,
                Selected = x.Id == directoryId
            });

            ViewData["ClientId"] = clientId;

            return PartialView(model);
        }

        [Authorize(policy: PolicyNames.StubPolicy)]
        public async Task<IActionResult> Pools(string poolId = null)
        {
            var client = base.Client;
            var pools = await _poolManager.GetDescendantPools(client?.SecurityPoolId, inclusive: true);

            var model = pools.Select(x => new SelectListItem
            {
                Value = x.PoolId,
                Text = x.Name,
                Selected = x.PoolId == poolId
            });

            ViewData["ClientId"] = client.Id;

            return PartialView(model);
        }


        [Authorize(policy: PolicyNames.StubPolicy)]
        public IActionResult Categories()
        {
            ViewData["Level"] = "Client";
            ViewData["OwnerId"] = Client.Id;
            ViewData["ClientId"] = Client.Id;

            // TODO: Refactor view to remove reference to siteId
            ViewData["SiteId"] = null;

            return View();
        }
    }
}
