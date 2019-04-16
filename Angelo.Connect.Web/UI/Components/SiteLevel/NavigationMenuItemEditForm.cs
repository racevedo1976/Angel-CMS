using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper.Extensions;

using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Common.Mvc.ActionResults;
using Angelo.Connect.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using Angelo.Connect.Models;

namespace Angelo.Connect.Web.UI.Components
{
    public class NavigationMenuItemEditForm : ViewComponent
    {
        NavigationMenuManager _navigationMenuManager;
        SiteAdminContext _siteAdminContext;

        public NavigationMenuItemEditForm(NavigationMenuManager navigationMenuManager, SiteAdminContext siteAdminContext)
        {
            _navigationMenuManager = navigationMenuManager;
            _siteAdminContext = siteAdminContext;
        }

        private List<SelectListItem> GetContentProviderSelectListItems()
        {
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Value = NavigationMenuItemType.Label, Text = "Label" });
            items.Add(new SelectListItem() { Value = NavigationMenuItemType.ExternalURL, Text = "External Link" });
            foreach (var provider in _navigationMenuManager.GetNavMenuContentProviders())
                items.Add(new SelectListItem() { Value = provider.Name, Text = provider.Title });
            return items;
        }

        private string GetContentTitle(string contentType, string contentId)
        {
            var provider = _navigationMenuManager.GetNavMenuContentProvider(contentType);
            if (provider == null)
                return string.Empty;
            var item = provider.GetContentItem(contentId);
            if (item == null)
                return string.Empty;
            return item.Title;
        }

        private string GetContentTypeLabel(string contentType)
        {
            var provider = _navigationMenuManager.GetNavMenuContentProvider(contentType);
            if (provider == null)
                return string.Empty;
            else
                return provider.Title;
        }

        public async Task<IViewComponentResult> InvokeAsync(string navMenuId = null, string parentId = null, string id = null, string siteId = null)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var menuItem = await _navigationMenuManager.GetNavMenuItemAsync(id);
                var model = menuItem.ProjectTo<NavigationMenuItemViewModel>();
                model.SiteId = _siteAdminContext.Site.Id;
                model.ContentTitle = GetContentTitle(model.ContentType, model.ContentId);
                model.ContentTypeLabel = GetContentTypeLabel(model.ContentType);
                model.ContentProviders = GetContentProviderSelectListItems();
                ViewData["FormTitle"] = "Edit Menu Item:";
                ViewData["CreateOrEdit"] = "edit";
                return View(model);
            }
            else if (!string.IsNullOrEmpty(navMenuId))
            {
                var model = new NavigationMenuItemViewModel();
                model.Id = string.Empty;
                model.SiteId = _siteAdminContext.Site.Id;
                model.NavMenuId = navMenuId;
                model.ParentId = parentId;
                model.ContentType = NavigationMenuItemType.Label;
                model.ContentProviders = GetContentProviderSelectListItems();
                ViewData["FormTitle"] = "Add Menu Item:";
                ViewData["CreateOrEdit"] = "create";
                return View(model);
            }
            else
                return new ViewComponentPlaceholder();
        }

    }

}
