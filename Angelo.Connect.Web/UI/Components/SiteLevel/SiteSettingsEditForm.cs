using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper.Extensions;

using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Connect.Services;
using Angelo.Connect.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Angelo.Common.Extensions;

namespace Angelo.Connect.Web.UI.Components
{
    public class SiteSettingsEditForm : ViewComponent
    {
        private SiteManager _siteManager;
        private PageManager _pageManager;

        public SiteSettingsEditForm(SiteManager siteManager, PageManager pageManager)
        {
            _siteManager = siteManager;
            _pageManager = pageManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string siteId)
        {
            SiteSettingsViewModel model = new SiteSettingsViewModel();
            IEnumerable<SelectListItem> pages = new List<SelectListItem>();
             
            if (!string.IsNullOrEmpty(siteId))
            {
                var settingsResult = await _siteManager.GetSiteSettingsAsync(siteId);
                var pagesResult = await _pageManager.GetPagesAsync(siteId);

                if (settingsResult != null && pagesResult != null)
                {
                    var dictionary = settingsResult.ToDictionary(x => x.FieldName, x => x.Value);
                    model = dictionary.ConvertTo<SiteSettingsViewModel>();

                    pages = pagesResult.Select(x => new SelectListItem
                    {
                        Value = x.Id,
                        Text = x.Title
                    });
                    pages = DefaultPage.Concat(pages);
                };
            }
            if (model == null)
                model = new SiteSettingsViewModel();

            ViewData["SiteId"] = siteId;
            ViewData["Pages"] = pages;

            return View(model);
        }

        private IEnumerable<SelectListItem> DefaultPage
        {
            get
            {
                return Enumerable.Repeat(new SelectListItem
                {
                    Value = "0",
                    Text = "--default--"
                }, count: 1);
            }
        }
    }

}
