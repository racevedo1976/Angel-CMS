﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Identity;
using Angelo.Connect.Security;
using System.Collections.Generic;

namespace Angelo.Connect.Web.UI.Components
{
    public class SiteUserPermissions : ViewComponent
    {
        private SiteManager _siteManager;
        private UserManager _userManager;
        private SecurityClaimManager _claimsManager;

        public SiteUserPermissions(SiteManager siteManager, UserManager userManager, SecurityClaimManager claimsManager)
        {
            _userManager = userManager;
            _siteManager = siteManager;
            _claimsManager = claimsManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string siteId, string userId)
        {
            var site = await _siteManager.GetByIdAsync(siteId);
          
            var userCurrentClaims = await _userManager.GetClaimObjectsAsync(userId);
            var permissionGroups = _claimsManager.GetAllPermissionForPoolId(site.SecurityPoolId);

            ViewData["SelectedClaims"] = userCurrentClaims;
            ViewData["PermissionGroups"] = ToKendoTreeViewModel(permissionGroups.ToList(), userCurrentClaims);
            // TODO - Ensure user's directory is mapped to this site's security pool

            ViewData["siteId"] = siteId;
            ViewData["PoolId"] = site.SecurityPoolId;
            ViewData["UserId"] = userId;

            return View();
        }

        public IList<SecurityPermissionViewModel> ToKendoTreeViewModel(IList<Permission> permissionList, IList<Identity.Models.UserClaim> claimsSelected)
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

                //hiding Personal Pages permissions for now
                if (permission.Title == "Create / Manage Personal Pages" || permission.Title == "Publish Personal Pages")
                {
                    //do not add
                }
                else
                {
                    //map and add
                    vmPermissionList.Add(new SecurityPermissionViewModel
                    {
                        Title = permission.Title,
                        Selected = preSelected,
                        Permissions = ToKendoTreeViewModel(permission.Permissions, claimsSelected)
                    });
                }
            }

            return vmPermissionList;
        }



    }
}
