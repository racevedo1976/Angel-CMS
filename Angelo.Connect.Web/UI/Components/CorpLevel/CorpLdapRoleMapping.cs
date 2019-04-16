using Angelo.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Identity.Models;

namespace Angelo.Connect.Web.UI.Components.CorpLevel
{
    public class CorpLdapRoleMapping : ViewComponent
    {
        RoleManager _roleManager;

        public CorpLdapRoleMapping(RoleManager roleManager)
        {
            _roleManager = roleManager;

        }

        public async Task<IViewComponentResult> InvokeAsync(string id = null)
        {
            var ldapMapping = new List<LdapMapping>();

            if (!string.IsNullOrEmpty(id))
            {
                var objectId = id.Split('_');
                var resourceType = objectId[0];
                var roleId = objectId[1];
                var poolId = objectId[2];
                var directoryId = objectId[3];

                ldapMapping = await _roleManager.GetLdapMappingAsync(roleId);

                ViewData["id"] = roleId;
                ViewData["poolId"] = poolId;
                ViewData["directoryId"] = directoryId;
            }
            return View(ldapMapping);
        }
    }
}
