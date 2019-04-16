using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace Angelo.Connect.Web.UI.Search.Components
{
    public class SearchInput : ViewComponent
    {
        private SiteContextAccessor _siteContextAccessor;

        public SearchInput(SiteContextAccessor siteContextAccessor)
        {
            _siteContextAccessor = siteContextAccessor;
        }


        public async Task<IViewComponentResult> InvokeAsync()
        {
            var siteContext = _siteContextAccessor.GetContext();
            var viewName = "LuceneInput";

            ViewData["SiteUrl"] = siteContext.DefaultUrl;

            if (siteContext.Options.ContainsKey("GoogleSiteId"))
            {
                viewName = "GoogleInput";

                ViewData["GoogleSiteId"] = siteContext.Options["GoogleSiteId"];
                ViewData["GoogleSearchCode"] = siteContext.Options["GoogleSearchCode"];                 
            }


            return await Task.FromResult(View($"~/UI/Search/Views/{viewName}.cshtml"));
        }
    }
}
