﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Common.Mvc.ActionResults;
using Angelo.Connect.Security;
using Angelo.Identity;
using Angelo.Connect.Extensions;
using System.Linq;

namespace Angelo.Connect.Web.UI.Components
{
    public class PoolRoleClaimListViewComponent : ViewComponent
    {
        private SecurityPoolManager _poolManager;
        private RoleManager _roleManager;
        private ClientManager _clientManager;
        private SecurityClaimManager _claimsManager;


        public PoolRoleClaimListViewComponent(ClientManager clientManager,
                                            RoleManager roleManager,
                                            SecurityPoolManager poolManager,
                                            SecurityClaimManager claimsManager)
        {
            _clientManager = clientManager;
            _roleManager = roleManager;
            _poolManager = poolManager;
            _claimsManager = claimsManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string poolId = "", string roleId = "")
        {
            if (string.IsNullOrEmpty(poolId) || string.IsNullOrEmpty(roleId))
                return new ViewComponentPlaceholder();

            //var poolClaims = _claimsManager.GetAllClaimsForPoolId(poolId);
            var role = await _roleManager.GetByIdAsync(roleId);
            var roleSelectedClaims = await _roleManager.GetClaimObjectsAsync(role);

            var permissionGroups = _claimsManager.GetAllPermissionForPoolId(poolId);

            //ViewData["ClaimList"] = poolClaims;
            ViewData["SelectedClaims"] = roleSelectedClaims;
            ViewData["PermissionGroups"] = ToKendoTreeViewModel(permissionGroups.ToList(), roleSelectedClaims);

            var model = new List<PoolRoleClaimViewModel>();
            ViewData["PoolId"] = poolId;
            ViewData["RoleId"] = roleId;
            return View(model);
        }

        /// <summary>
        ///     recursive call to build the kendo model for tree view.
        /// </summary>
        /// <param name="permissionList"></param>
        /// <param name="claimsSelected"></param>
        /// <returns></returns>
        public IList<SecurityPermissionViewModel> ToKendoTreeViewModel(IList<Permission> permissionList, IList<Identity.Models.RoleClaim> claimsSelected)
        {
            List<SecurityPermissionViewModel> vmPermissionList = new List<SecurityPermissionViewModel>();
            var preSelected = false;

            foreach (var permission in permissionList)
            {
                if (permission.Claims.Count() == 0)
                {
                    preSelected = false;
                }
                else
                {
                    
                    //assume that all claims are in the list first
                    bool groupSelected = true;

                    //double check that all claims in the permission group are selected
                    foreach (var claimInGroup in permission.Claims)
                    {
                        var x = claimsSelected.FirstOrDefault(c => c.ClaimType == claimInGroup.Type &&
                                                                  c.ClaimValue == claimInGroup.Value);
                        if (x == null)
                        {
                            groupSelected = false;
                        }
                    }

                    preSelected = groupSelected;


                }
                
                //map and add
                vmPermissionList.Add(new SecurityPermissionViewModel
                {
                    Title = permission.Title,
                    Selected = preSelected,
                    Permissions = ToKendoTreeViewModel(permission.Permissions, claimsSelected)
                });
            }

            return vmPermissionList;
        }



    }


}
