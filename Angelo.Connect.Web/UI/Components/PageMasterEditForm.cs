using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper.Extensions;

using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc.Rendering;
using Angelo.Connect.Configuration;

namespace Angelo.Connect.Web.UI.Components
{
    public class PageMasterEditForm : ViewComponent
    {
        private PageMasterManager _pageMasterManager;
        private SiteContext _siteContext;

        public PageMasterEditForm(PageMasterManager pageMasterManager, SiteContext siteContext)
        {
            _pageMasterManager = pageMasterManager;
            _siteContext = siteContext;
        }

        public async Task<IViewComponentResult> InvokeAsync(string siteId = "", string pageMasterId = "")
        {
            PageMasterViewModel model = null;

            if (string.IsNullOrEmpty(pageMasterId))
            {
                model = new PageMasterViewModel { SiteId = siteId };
            }
            else
            {
                var pageMaster = await _pageMasterManager.GetByIdAsync(pageMasterId);
                model = pageMaster.ProjectTo<PageMasterViewModel>();
            }

            var dropDownData = _siteContext.Template.PageTemplates;
           
            foreach (var item in dropDownData)
            {
                model.ViewTemplates.Add(new SelectListItem() { Value = item.Id, Text = item.Title });
            }

            return View(model);
        }
    }
}
