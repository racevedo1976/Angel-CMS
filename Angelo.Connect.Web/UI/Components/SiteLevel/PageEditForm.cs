using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper.Extensions;

using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc.Rendering;
using Angelo.Connect.Models;
using Angelo.Common.Mvc.ActionResults;

namespace Angelo.Connect.Web.UI.Components
{
    public class PageEditForm : ViewComponent
    {
        private PageManager _pageManager;
        private PageMasterManager _pageMasterManager;
        private SiteManager _siteManager;

        public PageEditForm(PageManager pageManager, PageMasterManager pageMasterManager, SiteManager siteManager)
        {
            _pageManager = pageManager;
            _pageMasterManager = pageMasterManager;
            _siteManager = siteManager;
        }

        private async Task<SelectList> GetMasterPagesSelectList(string siteId)
        {
            var result = await _pageMasterManager.GetMasterPagesAsync(siteId);
            var masterPages = result.Select(x => new { Id = x.Id, Value = x.Title });
            return new SelectList(masterPages, "Id", "Value");
        }

        public async Task<IViewComponentResult> CreateAsync(string siteId, string parentPageId)
        {
            var model = new PageViewModel()
            {
                SiteId = siteId
            };

            var siteDomain = await _siteManager.GetDefaultDomainWithProtocolAsync(siteId);

            model.Path = "";
            model.MasterPages = await GetMasterPagesSelectList(siteId);
            model.DefaultDomain = siteDomain.DomainKey;

            if (!string.IsNullOrEmpty(parentPageId))
            {
                var parentPage = await _pageManager.GetByIdAsync(parentPageId);

                model.ParentPageId = parentPage?.Id;
                model.ParentPageTitle = parentPage?.Title;
            }

            return View(model);
        }

        public async Task<IViewComponentResult> EditAsync(string pageId)
        {
            var page = await _pageManager.GetByIdAsync(pageId);
            if (page == null)
                return new ViewComponentPlaceholder();

            var model = page.ProjectTo<PageViewModel>();
            var siteDomain = await _siteManager.GetDefaultDomainWithProtocolAsync(page.SiteId);

            model.MasterPages = await GetMasterPagesSelectList(page.SiteId);
            model.Versions = await _pageManager.GetVersions(pageId);
            model.DefaultDomain = siteDomain.DomainKey;

            if(model.ParentPageId != null)
            {
                var parentPage = await _pageManager.GetByIdAsync(model.ParentPageId);

                model.ParentPageId = parentPage?.Id;
                model.ParentPageTitle = parentPage?.Title;
                model.PageMasterId = parentPage?.PageMasterId;
            }

            return View(model);
        }

        public async Task<IViewComponentResult> InvokeAsync(string siteId = null, string pageId = null, string parentId = null)
        {
            if (string.IsNullOrEmpty(pageId) == false)
                return await EditAsync(pageId);

            if (string.IsNullOrEmpty(siteId) == false)
                return await CreateAsync(siteId, parentId);

            return new ViewComponentPlaceholder();
        }

    }

}
