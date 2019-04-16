using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AutoMapper.Extensions;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

using Angelo.Common.Messaging;
using Angelo.Identity;
using Angelo.Identity.Models;
using Angelo.Identity.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Connect.Services;
using Angelo.Connect.Models;
using Angelo.Connect.Security;
using Angelo.Connect.Extensions;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Documents;

namespace Angelo.Connect.Web.UI.Controllers
{
    public class SiteUsersDataController : SiteControllerBase
    {
        private UserManager _userManager;
        private ClientManager _clientManager;
        private SecurityPoolManager _poolManager;
        private DirectoryManager _directoryManager;
        private TemplateEmailService _messaging;
        private AegisOptions _openIdOptions;
        private SecurityClaimManager _claimsManager;
        private IFolderManager<FileDocument> _folderManager;
        private ILogger<SiteUsersDataController> _logger;

        public SiteUsersDataController(
            UserManager userManager,
            Identity.SecurityPoolManager poolManager,
            DirectoryManager directoryManager,
            ClientManager clientManager,
            TemplateEmailService messaging,
            IOptions<AegisOptions> openIdOptions,
            SiteAdminContextAccessor siteContextAccessor,
            ILogger<SiteUsersDataController> logger,
            SecurityClaimManager claimsManager,
            IFolderManager<FileDocument> folderManager
        ) 
        : base(siteContextAccessor, logger)
        {
            _userManager = userManager;
            _poolManager = poolManager;
            _directoryManager = directoryManager;
            _clientManager = clientManager;
            _messaging = messaging;
            _openIdOptions = openIdOptions.Value;
            _claimsManager = claimsManager;
            _folderManager = folderManager;
            _logger = logger;
        }


        [Authorize(policy: PolicyNames.SiteUsersRead)]
        [HttpPost, Route("/sys/sites/{tenant}/api/users/data")]
        public async Task<IActionResult> GetUserData([DataSourceRequest]DataSourceRequest request, string directoryId = null)
        {
            var valid = await _directoryManager.IsMapped(directoryId, Site.SecurityPoolId);

            if (!valid)
                return BadRequest("Invalid or Unmapped Site Directory");

        

            var memberships = await _directoryManager.GetUsersAsync(directoryId);
            var model = memberships.ProjectTo<UserViewModel>();

            return Json(model.ToDataSourceResult(request));
        }

        [Authorize(policy: PolicyNames.SiteUsersCreate)]
        [HttpPost, Route("/sys/sites/{tenant}/api/users")]
        public async Task<IActionResult> CreateUser(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var valid = await _directoryManager.IsMapped(model.DirectoryId, Site.SecurityPoolId);

                if (!valid)
                    return BadRequest("Invalid or Unmapped Site Directory");

                var user = model.ProjectTo<User>();
                var result = await _userManager.CreateAsync(user);

                if (!result.Succeeded)
                    return BadRequest(String.Join(". ", result.Errors.Select(x => x.Description).ToArray()));

                //Ensure user has a library created
                var locationResolver = new DocumentPhysicalLocationResolver("User", Site.ClientId, "", user.Id);

                var userLibrary = await _folderManager.CreateDocumentLibrary(user.Id, "User", locationResolver.Resolve());

                //TODO. If success, then send email to user. Email user to set password.
                //MDJ: Use Try Catch so that this part doesn't blow up dev environment when not on VPN
                try
                {
                    await SendNewUserSetPasswordEmailAsync(user, Site.ClientId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            
                return Ok(user);
            }

            return BadRequest(ModelState);
        }

        [Authorize(policy: PolicyNames.SiteUsersEdit)]
        [HttpPut, Route("/sys/sites/{tenant}/api/users")]
        public async Task<IActionResult> UpdateUser(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var valid = await _directoryManager.IsMapped(model.DirectoryId, Site.SecurityPoolId);

                if (!valid)
                    return BadRequest("Invalid or Unmapped Site Directory");

                var user = model.ProjectTo<User>();             
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                    return BadRequest(String.Join(". ", result.Errors.Select(x => x.Description).ToArray()));

                return Ok();
            }

            return BadRequest(ModelState);
        }

        [Authorize(policy: PolicyNames.SiteUsersRead)]
        [HttpGet, Route("/sys/sites/{tenant}/api/users/{userId}/roles")]
        public async Task<ActionResult> GetUserRoleData(string userId)
        {
            var poolId = Site.SecurityPoolId;
            var userRoles = await _poolManager.GetUserRolesAsync(poolId, userId);

            return Json(userRoles);
        }

        [Authorize(policy: PolicyNames.SiteUsersEdit)]
        [HttpPost, Route("/sys/sites/{tenant}/api/users/{userId}/roles")]
        public async Task<IActionResult> UpdateUserRoles(string userId, [FromForm]string[] roles)
        {
            var poolId = Site.SecurityPoolId;

            if (!string.IsNullOrEmpty(userId))
            {
                var result = await _poolManager.SetUserRolesAsync(poolId, userId, roles);

                if (!result.Succeeded)
                    return BadRequest(String.Join(". ", result.Errors.Select(x => x.Description).ToArray()));

                return Ok();
            }

            return BadRequest("Invalid parameter: userId");
        }
       
        // TODO: MOve this to user manager
        private async Task SendNewUserSetPasswordEmailAsync(User user, string clientId)
        {
            
            //TODO: Refactor "purpose" to be a global property
            var token = await _userManager.GenerateUserTokenAsync(user,nameof(ConnectCustomTokenProvider<User>), "NewUserPassword");

            var tenantKey = (await _clientManager.GetByIdAsync(clientId)).TenantKey;
            
            var link = string.Format(_openIdOptions.EndPoints.SetPassword, tenantKey, user.Id, WebUtility.UrlEncode(token), WebUtility.UrlEncode($"{Request.Scheme}://{Request.Host.Value}"));
            
            var newUserSetPassword = new NewUserSetPassword()
            {
                Username = user.UserName,
                EmailLink = link
            };
            
            await _messaging.SendEmailAsync(user.Email, newUserSetPassword);
        }

        [Authorize(policy: PolicyNames.SiteUsersEdit)]
        [HttpPost("/sys/sites/{tenant}/api/users/{userId}/permissions")]
        public async Task<ActionResult> Permissions(SecurityUserPermissionsViewModel model, string userId)
        {
            if (ModelState.IsValid)
            {
                // prevent bypassing security
                if (model.UserId != userId || model.PoolId != Site.SecurityPoolId)
                    return BadRequest("Model is invalid for this route");


                IdentityResult result;

                //to avoid removing claims in other groups we first tackle the groups not selected or ones deselected, remove those claims first.
                //then add claims  from selected permissions group. Performing 2 loops will take care of that.
                // I wish there was a better way....


                //Get all permissions/claims under the Pool id in context
                var poolPermissions = _claimsManager.GetAllPermissionForPoolId(model.PoolId);
                var poolPermissionClaims = GetAllClaimsFromPermissions(poolPermissions);

                //easiest way so far...
                //loop to start removing all claims that whether selected or deselected. This will clear the role claims for this group and let use start adding new ones.
                foreach (var poolPermissionClaim in poolPermissionClaims)
                {
                    result = await _userManager.RemoveClaimAsync(poolPermissionClaim.Type, poolPermissionClaim.Value, model.UserId);
                    //if (!result.Succeeded)
                    //    return ErrorView(result.Errors.ToString());
                }


                //Start Adding permission groups claims that were selected.
                foreach (var selectedPermission in model.SelectedPermissionGroups)
                {
                    //get specific claims for this selected permissionGroup (to be added)
                    var claimsForSelectedPermission = poolPermissions.FindClaims(selectedPermission).Distinct();

                    //start adding each claim for this permission to the role, if not already there.
                    foreach (var securityClaim in claimsForSelectedPermission)
                    {
                        //find that Claim doesnt exist in this role first. If it does then skip, if doesn't then add.
                        if (_userManager.QueryUserClaims().FirstOrDefault(x => x.UserId == model.UserId
                                                                                            && x.ClaimType == securityClaim.Type
                                                                                            && x.ClaimValue == securityClaim.Value) == null)
                        {
                            var claim = new System.Security.Claims.Claim(securityClaim.Type, securityClaim.Value);
                            result = await _userManager.AddClaimAsync(userId, claim);
                        }

                    }

                }

                return Ok();
            }
            return BadRequest(ModelState);
        }

        [Authorize(policy: PolicyNames.SiteUsersEdit)]
        [HttpPost, Route("/sys/sites/{tenant}/api/users/{userId}/confirm")]
        public async Task<IActionResult> ConfirmUserEmail(string userId)
        {
            var user = await _userManager.GetUserAsync(userId);

            try
            {
                await SendNewUserSetPasswordEmailAsync(user, Site.ClientId);
                return Ok("success");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }

        private IList<SecurityClaim> GetAllClaimsFromPermissions(IEnumerable<Permission> poolPermissions)
        {
            var claimList = new List<SecurityClaim>();

            foreach (var poolPermission in poolPermissions)
            {
                foreach (var claim in poolPermission.Claims)
                {
                    claimList.Add(claim);
                }

                if (poolPermission.Permissions.Count() > 0)
                {
                    claimList.AddRange(GetAllClaimsFromPermissions(poolPermission.Permissions));
                }
            }

            return claimList;
        }

    }
}
