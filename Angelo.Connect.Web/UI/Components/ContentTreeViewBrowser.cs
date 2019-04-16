using Angelo.Connect.Abstractions;
using Angelo.Connect.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Connect.Web.UI.ViewModels.Extensions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Extensions;

namespace Angelo.Connect.Web.UI.Components
{
    public class ContentTreeViewBrowser : ViewComponent
    {
        private IEnumerable<IContentBrowser> _browserTypes;
        private SiteContext _siteContext;

        public ContentTreeViewBrowser(IEnumerable<IContentBrowser> browserTypes, IContextAccessor<SiteContext> siteContextAccessor)
        {
            _browserTypes = browserTypes;
            _siteContext = siteContextAccessor.GetContext();

           
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId, string expandNodeId)
        {
            IList<TreeView> treeViewModel = new List<TreeView>();
            List<TreeView> sharedTreeViewModel = new List<TreeView>();

            //Get Product Definition flag for sharing Documents
            var isSiteDocumentSharingEnabled = _siteContext.ProductContext.Features.Get(FeatureId.DocumentSharing)?.GetSettingValue<bool>("enabled") ?? false;

            //Get content type nodes to display on the tree
            foreach (var contentBrowserProvider in _browserTypes)
            {
                
                //Get Content Types Contents to Display in TreeView - recursive - based on context security
                if (await contentBrowserProvider.HasAccessToLibrary())
                    treeViewModel.Add(await contentBrowserProvider.GetRootTreeView(userId));
            }

            //Get all Share Content
            if (isSiteDocumentSharingEnabled)
            {
                foreach (var contentBrowserProvider in _browserTypes)
                {
                    //Get Specific Share Content Items with User
                    if (await contentBrowserProvider.IsAnythingShared(userId))
                    {
                        sharedTreeViewModel.Add(contentBrowserProvider.GetSharedContentTreeNode());
                    }
                }
            }
           

            if (sharedTreeViewModel.Any())
            {
                treeViewModel.Add(new TreeView
                {
                    Title = "Shared With Me",
                    Id = "shareableroot",
                    IconClass = "",
                    ContentBrowserType = "test",
                    Items = sharedTreeViewModel
                });
            }



            ViewData["ExpandNodeId"] = expandNodeId;

          

            return View(treeViewModel);
           
        }

      
    }
}
