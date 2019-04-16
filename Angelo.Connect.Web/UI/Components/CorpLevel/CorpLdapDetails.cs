using Angelo.Identity.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Identity.Models;

namespace Angelo.Connect.Web.UI.Components.CorpLevel
{
    public class CorpLdapDetails : ViewComponent
    {
        DirectoryManager _directoryManager;
        LdapManager _ldapManager;
        public CorpLdapDetails(DirectoryManager directoryManager, LdapManager ldapManager)
        {
            _directoryManager = directoryManager;
            _ldapManager = ldapManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id = null)
        {

            //var x = _ldapManager.SearchForGroup("MPCS_Groups");


            var viewPage = "Default";
            var ldap = new LdapDomain();

            if (!string.IsNullOrEmpty(id))
            {
                var objectId = id.Split('_');
                var resourceType = objectId[0];
                var directoryId = objectId[1];

                ldap = await _directoryManager.GetDirectoryLdapAsync(directoryId) ?? new LdapDomain() { DirectoryId = directoryId};

                ViewData["id"] = directoryId;
            }

            return View("CorpLdapConnection", ldap);
        }
    }
}
