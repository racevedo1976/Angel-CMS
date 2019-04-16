using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper.Extensions;

using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Kendo.Mvc.UI;
using Angelo.Connect.Models;
using Angelo.Common.Mvc.ActionResults;

namespace Angelo.Connect.Web.UI.Components
{
    public class NavigationMenuEditForm : ViewComponent
    {
        private NavigationMenuManager _navigationMenuManager;

        public NavigationMenuEditForm(NavigationMenuManager navigationMenuManager)
        {
            _navigationMenuManager = navigationMenuManager;
        }

        private void LoadItemIntoNode(NavigationMenuItem item, TreeViewItemModel node)
        {
            node.Id = item.Id;
            node.Text = item.Title;
            node.HasChildren = (item.Children.Count > 0);
            //node.Expanded = node.HasChildren;
            node.Enabled = true;
            foreach (var child in item.Children)
            {
                var childNode = new TreeViewItemModel();
                LoadItemIntoNode(child, childNode);
                node.Items.Add(childNode);
            }
        }

        private bool ExpandToNode(string itemId, List<TreeViewItemModel> sourceItems)
        {
            foreach(var node in sourceItems)
            {
                if (node.Id == itemId)
                {
                    node.Expanded = false;
                    return true;
                }
                else if (ExpandToNode(itemId, node.Items))
                {
                    node.Expanded = true;
                    return true;
                }
            }
            return false;
        }

        public async Task<IViewComponentResult> InvokeAsync(string navMenuId, string itemId = null)
        {
            if (string.IsNullOrEmpty(navMenuId))
                return new ViewComponentPlaceholder();

            var navMenu = await _navigationMenuManager.GetFullNavMenuAsync(navMenuId);
            if (navMenu == null)
                return new ViewComponentPlaceholder();

            var model = new NavigationMenuViewModel();
            model.Id = navMenu.Id;
            model.SiteId = navMenu.SiteId;
            model.Title = navMenu.Title;
            foreach (var item in navMenu.MenuItems)
            {
                var node = new TreeViewItemModel();
                LoadItemIntoNode(item, node);
                model.Items.Add(node);
            }
            if (!string.IsNullOrEmpty(itemId))
                ExpandToNode(itemId, model.Items);

            ViewData["FormTitle"] = "Edit Navigation Menu:";
            return View(model);
        }


    }
}

