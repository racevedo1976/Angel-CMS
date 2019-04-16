using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

using Angelo.Connect.Models;
using Angelo.Connect.Services;
using Angelo.Connect.Security;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Identity;
using AutoMapper.Extensions;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Web.UI.Controllers.Api
{
    public class NavigationMenuDataController : SiteControllerBase
    {
        private NavigationMenuManager _navigationMenuManager;

        public NavigationMenuDataController(
            NavigationMenuManager navigationMenuManager,
            ILogger<SecurityPoolManager> logger,
            SiteAdminContextAccessor siteContextAccessor
        ) : base(siteContextAccessor, logger)
        {
            _navigationMenuManager = navigationMenuManager;
        }

        [Authorize(policy: PolicyNames.SiteNavMenusRead)]
        [HttpPost, Route("/sys/sites/{tenant}/api/navigationmenuitems/data")]
        public async Task<JsonResult> GetNavMenusOfSiteId([DataSourceRequest] DataSourceRequest request)
        {
            var siteId = Site.Id;
            var query = _navigationMenuManager.GetNavMenusOfSiteIdQuery(siteId);
            var result = query.ToDataSourceResult(request);
            result.Data = result.Data.ProjectTo<NavigationMenuViewModel>();
            return Json(result);
        }

        [Authorize(policy: PolicyNames.SiteNavMenusCreate)]
        [HttpPut, HttpPost, Route("/sys/sites/{tenant}/api/navigationMenu")]
        public async Task<JsonResult> SaveNavMenu(NavigationMenuViewModel vm)
        {
            var navMenu = new NavigationMenu();
            navMenu.Id = vm.Id;
            navMenu.Title = vm.Title;
            navMenu.SiteId = vm.SiteId;

            navMenu = await _navigationMenuManager.SaveNavMenuAsync(navMenu);
            vm.Id = navMenu.Id;

            return Json(vm);
        }

        [Authorize(policy: PolicyNames.SiteNavMenusDelete)]
        [HttpDelete, Route("/sys/sites/{tenant}/api/navigationmenu/{id}")]
        public async Task<JsonResult> DeleteNavMenu(string id)
        {
            await _navigationMenuManager.DeleteNavMenuAsync(id);
            return Json(Ok());
        }

        [Authorize(policy: PolicyNames.SiteNavMenusEdit)]
        [HttpPost, Route("/sys/sites/{tenant}/api/navigationmenuitems/move")]
        public async Task<JsonResult> MoveNavMenuItem(string sourceId, string destId, string dropPosition)
        {
            await _navigationMenuManager.MoveNavMenuItemRelativeToDestItem(sourceId, destId, dropPosition);
            return Json(Ok());
        }

        [Authorize(policy: PolicyNames.SiteNavMenusEdit)]
        [HttpPost, Route("/sys/sites/{tenant}/api/navigationmenuitems")]
        public async Task<ActionResult> SaveNavMenuItem(NavigationMenuItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                var navMenuItem = model.ProjectTo<NavigationMenuItem>();
                if (string.IsNullOrEmpty(navMenuItem.Id))
                {
                    var newItem = await _navigationMenuManager.InsertNavMenuItemAsync(navMenuItem);
                    model.Id = newItem.Id; 
                }
                else
                    await _navigationMenuManager.UpdateNavMenuItemAsync(navMenuItem);
                return Ok(model);
            }
            return BadRequest(ModelState);
        }

        [Authorize(policy: PolicyNames.SiteNavMenusEdit)]
        [HttpDelete, Route("/sys/sites/{tenant}/api/navigationmenuitems/{id}")]
        public async Task<JsonResult> DeleteNavMenuItem(string id)
        {
            await _navigationMenuManager.DeleteMenuItemAsync(id);
            return Json(Ok());
        }

        [Authorize(policy: PolicyNames.SiteNavMenusRead)]
        [HttpPost, Route("/sys/sites/{tenant}/api/navigationmenuitems/content")]
        public async Task<JsonResult> GetNavMenuItemContent([DataSourceRequest] DataSourceRequest request, string contentType, string id = null)
        {
            var a = Request;
            var siteId = Site.Id;

            IEnumerable<NavMenuItemContent> content;
            var provider = _navigationMenuManager.GetNavMenuContentProvider(contentType);
            if (provider == null)
                content = new List<NavMenuItemContent>();
            else if (string.IsNullOrEmpty(id))
                content = provider.GetRootItems(siteId);
            else
            {
                // There is no way to pass the correct contentType when the TreeView expands a node.  So, get it from the parent node.
                //provider = _navigationMenuManager.GetNavMenuContentProviderOfContentId(id);
                //if (provider == null)
                //    content = new List<NavMenuItemContent>();
                //else
                    content = provider.GetChildItems(siteId, id);
            }

            var list = content.Select(x => new {
                id = x.Id,
                name = x.Title,
                description = x.Description,
                hasChildren = x.HasChildren
            }).ToList();

            return Json(list);
        }
    }
}
