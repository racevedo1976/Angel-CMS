using Angelo.Connect.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.Components
{
    public class ContentListViewBrowser : ViewComponent
    {
        private IEnumerable<IContentBrowser> _browserTypes;

        public ContentListViewBrowser(IEnumerable<IContentBrowser> browserTypes)
        {
            _browserTypes = browserTypes;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId, 
            string providerName, string contentId, bool isShared,
            string onSelect)
        {
            string componentView;

            var provider = _browserTypes.FirstOrDefault(x => x.GetType().Name == providerName);
            if (provider == null)
            {
                componentView = "default";
            }
            else
            {
                componentView = provider.GetComponentContentView();
                if (contentId.Contains("sharedcontent"))
                    ViewData["model"] = await provider.GetSharedContent(userId);
                else
                    ViewData["model"] = await provider.GetContent(contentId);
            }

            ViewData["contentId"] = contentId;
            ViewData["onSelect"] = onSelect;
            ViewData["isShared"] = isShared;

            return View(componentView);
        }
    }
}
