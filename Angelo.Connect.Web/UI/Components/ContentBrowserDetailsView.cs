using Angelo.Connect.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.Components
{
    public class ContentBrowserDetailsView : ViewComponent
    {
        private IEnumerable<IContentBrowser> _browserTypes;

        public ContentBrowserDetailsView(IEnumerable<IContentBrowser> browserTypes)
        {
            _browserTypes = browserTypes;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId, string selectedContentId, bool isShared)
        {
            var componentView = "default";
            var contentId = "";
            var rootLibraryName = "";
            var canManageLibrary = false;
            var providerName = "";

            //the selectedContentId has the following pattern:
            //   [IContentBrowser]_[Id]
            //where IContentBrowser is the name type of the provider and the ID is the item selected to get the details from
            if (!string.IsNullOrEmpty(selectedContentId)  && selectedContentId != "0")
            {
                var idContext = selectedContentId.Split('_');
                providerName = idContext[0];
                contentId = idContext[1];

                var provider = _browserTypes.FirstOrDefault(x => x.GetType().Name == providerName);

                if (contentId.Contains("sharedcontent"))
                {
                    ViewData["model"] = await provider.GetSharedContent(userId);
                }
                else
                {
                    ViewData["model"] = await provider.GetContent(contentId);
                }

                componentView = provider.GetComponentContentView();
                rootLibraryName = provider.GetLibraryRootName();
                canManageLibrary = await provider.CanManageLibrary();
            }
            else
            {
                ViewData["model"] = null;
            }

            ViewData["contentId"] = contentId;
            ViewData["isShared"] = isShared;
            ViewData["rootFolderName"] = rootLibraryName;
            ViewData["canManageLibrary"] = canManageLibrary;
            ViewData["userId"] = userId;
            ViewData["providerName"] = providerName;
            return View(componentView);

        }
    }
}
