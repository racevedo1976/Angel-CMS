using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using AutoMapper.Extensions;

using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Connect.Extensions;
using Angelo.Connect.Security;
using Angelo.Identity;
using Angelo.Identity.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Angelo.Connect.Web.UI.Controllers.Api.SiteLevel
{
    public class SiteRolesDataController : SiteControllerBase
    {
        private RoleManager _roleManager;
        private SecurityClaimManager _claimsManager;
        private SecurityPoolManager _poolManager;

        public SiteRolesDataController(
            RoleManager roleManager,
            SecurityClaimManager claimsManager,
            SecurityPoolManager poolManager,
            SiteAdminContextAccessor siteContextAccessor,
            ILogger<SiteRolesDataController> logger

        ) : base(siteContextAccessor, logger)
        {
            _roleManager = roleManager;
            _poolManager = poolManager;
            _claimsManager = claimsManager;
        }

        [Authorize(policy: PolicyNames.SiteRolesRead)]
        [ResourceContext(routeKey: null)]
        [HttpPost("/sys/sites/{tenant}/api/roles/data")]
        public async Task<ActionResult> GetRoles([DataSourceRequest]DataSourceRequest request)
        {
            var poolId = Site.SecurityPoolId;
            var roles = await _poolManager.GetRolesAsync(poolId);
            var model = roles.ProjectTo<RoleViewModel>();

            return Json(model.ToDataSourceResult(request));
        }

        [Authorize(policy: PolicyNames.SiteRolesCreate)]
        [HttpPost("/sys/sites/{tenant}/api/roles")]
        public async Task<ActionResult> CreateRole(PoolRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = model.ProjectTo<Role>();
                IdentityResult result;

                if (string.IsNullOrEmpty(model.Id))
                    result = await _roleManager.CreateAsync(role);
                else
                    result = await _roleManager.UpdateAsync(role);

                if (!result.Succeeded)
                    return BadRequest(String.Join(". ", result.Errors.Select(x => x.Description).ToArray()));

                return Ok(model);
            }
            return BadRequest(ModelState);
        }

        [Authorize(policy: PolicyNames.SiteRolesEdit)]
        [HttpPut("/sys/sites/{tenant}/api/roles/{roleId}")]
        public async Task<ActionResult> UpdateRole(PoolRoleViewModel model, string roleId)
        {
            if (ModelState.IsValid && roleId == model.Id)
            {
                var role = model.ProjectTo<Role>();
                IdentityResult result;

                result = await _roleManager.UpdateAsync(role);

                if (!result.Succeeded)
                    return BadRequest(String.Join(". ", result.Errors.Select(x => x.Description).ToArray()));

                return Ok(model);
            }
            return BadRequest(ModelState);
        }

        [Authorize(policy: PolicyNames.SiteRolesDelete)]
        [HttpDelete, Route("/sys/sites/{tenant}/api/roles/{roleId}")]
        public async Task<ActionResult> DeleteRole(string roleId)
        {
            if (roleId != null)
            {
                var result = await _roleManager.DeleteAsync(roleId);

                if (!result.Succeeded)
                    return BadRequest(String.Join(". ", result.Errors.Select(x => x.Description).ToArray()));

                return Ok();
            }

            return BadRequest();
        }

        [Authorize(policy: PolicyNames.SiteRolesEdit)]
        [HttpPost("/sys/sites/{tenant}/api/roles/{roleId}/permissions/add")]
        public async Task<ActionResult> PermissionsAdd(SecurityPermissionRoleViewModel model, string roleId)
        {
            if (ModelState.IsValid)
            {
                // prevent bypassing security
                if (model.RoleId != roleId || model.PoolId != Site.SecurityPoolId)
                    return BadRequest("Model is invalid for this route");

                // checking for null or locked roles
                var role = await _roleManager.GetByIdAsync(roleId);

                if (role == null)
                    return BadRequest("Invalid or missing role");

                if (role.IsLocked)
                    return BadRequest("Cannot edit the permissions of a locked role");

                IdentityResult result;

                //Get all claims under the permissions group
                var permissions = _claimsManager.GetAllPermissionForPoolId(model.PoolId);
                var claims = permissions.FindClaims(model.PermissionTitle).Distinct();
                var roleSelectedClaims = await _roleManager.GetClaimObjectsAsync(role);

                foreach (var securityClaim in claims)
                {
                    var claim = new System.Security.Claims.Claim(securityClaim.Type, securityClaim.Value);

                    //check if claim is in role...
                    if (roleSelectedClaims.Any())
                    {
                        if (roleSelectedClaims.Any(c => c.ClaimType == securityClaim.Type
                                                    && c.ClaimValue == securityClaim.Value))
                        {
                            continue;
                        }
                    }
                    result = await _roleManager.AddClaimAsync(new Role() { Id = role.Id }, claim);
                }


                return Ok(model);
            }
            return BadRequest(ModelState);
        }

        [Authorize(policy: PolicyNames.SiteRolesEdit)]
        [HttpPost("/sys/sites/{tenant}/api/roles/{roleId}/permissions/remove")]
        public async Task<ActionResult> PermissionsRemove(SecurityPermissionRoleViewModel model, string roleId)
        {
            if (ModelState.IsValid)
            {
                // prevent bypassing security
                if (model.RoleId != roleId || model.PoolId != Site.SecurityPoolId)
                    return BadRequest("Model is invalid for this route");

                // checking for null or locked roles
                var role = await _roleManager.GetByIdAsync(roleId);

                if (role == null)
                    return BadRequest("Invalid or missing role");

                if (role.IsLocked)
                    return BadRequest("Cannot edit the permissions of a locked role");

                IdentityResult result;

                //Get all claims under the permissions group
                var permissions = _claimsManager.GetAllPermissionForPoolId(model.PoolId);
                var claims = permissions.FindClaims(model.PermissionTitle).Distinct();

                foreach (var securityClaim in claims)
                {

                    result = await _roleManager.RemoveClaimAsync(securityClaim.Type, securityClaim.Value, model.RoleId);
                    if (!result.Succeeded)
                        return ErrorView(result.Errors.ToString());
                }

                return Ok(model);

            }
            return BadRequest(ModelState);
        }

        [Authorize(policy: PolicyNames.SiteRolesEdit)]
        [HttpPost("/sys/sites/{tenant}/api/roles/{roleId}/permissions")]
        public async Task<ActionResult> Permissions(SecurityRolePermissionsViewModel model, string roleId)
        {
            if (ModelState.IsValid)
            {
                // prevent bypassing security
                if (model.RoleId != roleId || model.PoolId != Site.SecurityPoolId)
                    return BadRequest("Model is invalid for this route");

                // checking for null or locked roles
                var role = await _roleManager.GetByIdAsync(roleId);

                if (role == null)
                    return BadRequest("Invalid or missing role");

                if (role.IsLocked)
                    return BadRequest("Cannot edit the permissions of a locked role");

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
                    result = await _roleManager.RemoveClaimAsync(poolPermissionClaim.Type, poolPermissionClaim.Value, model.RoleId);
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
                        if (_roleManager.QueryRoleClaims().FirstOrDefault(x => x.RoleId == model.RoleId
                                                                                            && x.ClaimType == securityClaim.Type
                                                                                            && x.ClaimValue == securityClaim.Value) == null)
                        {
                            var claim = new System.Security.Claims.Claim(securityClaim.Type, securityClaim.Value);
                            result = await _roleManager.AddClaimAsync(new Role() { Id = role.Id }, claim);
                        }

                    }

                }

                return Ok();
            }
            return BadRequest(ModelState);
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
