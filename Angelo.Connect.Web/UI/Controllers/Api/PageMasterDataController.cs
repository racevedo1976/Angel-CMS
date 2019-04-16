using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

using Angelo.Connect.Models;
using Angelo.Connect.Services;
using Angelo.Connect.Security;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using AutoMapper.Extensions;
using Angelo.Connect.Configuration;

namespace Angelo.Connect.Web.UI.Controllers.Api
{
    public class PageMasterDataController : Controller
    {
        private PageMasterManager _pageMasterManager;
        private PageManager _pageManager;
        private SiteManager _siteManager;
        private SiteContext _siteContext;
        private ConnectCoreOptions _connectOptions;

        public PageMasterDataController(PageMasterManager pageMasterManager, PageManager pageManager, SiteManager siteManager,
            SiteContext siteContext, ConnectCoreOptions connectOptions)
        {
            _pageMasterManager = pageMasterManager;
            _pageManager = pageManager;
            _siteManager = siteManager;
            _siteContext = siteContext;
            _connectOptions = connectOptions;
        }

        [Authorize(policy: PolicyNames.SiteMasterPagesRead)]
        [HttpPost, Authorize, Route("/api/masterpages/data")]
        public async Task<JsonResult> Data([DataSourceRequest] DataSourceRequest request, string siteId)
        {
            var masterPages = await _pageMasterManager.GetMasterPagesAsync(siteId);
            var model = masterPages.ProjectTo<PageMasterViewModel>();


            return Json(model.ToDataSourceResult(request));
        }

        [Authorize(policy: PolicyNames.SiteMasterPagesRead)]
        [HttpGet, Route("/sys/sites/{tenant}/api/masterpages/data/{id}")]
        public async Task<JsonResult> GetById(string Id)
        {
            var masterPage = await _pageMasterManager.GetByIdAsync(Id);

            return Json(masterPage);
        }

        [Authorize(policy: PolicyNames.SiteMasterPagesEdit)]
        [HttpPut, Route("/api/masterpages")]
        public async Task<ActionResult> Update(PageMasterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var pageMaster = model.ProjectTo<PageMaster>();

                if (string.IsNullOrEmpty(model.Id))
                {
                    var id = await _pageMasterManager.CreateAsync(pageMaster);
                    model.Id = id;
                }
                else
                {
                    pageMaster.TemplateId = model.TemplateId;
                    await _pageMasterManager.UpdateAsync(pageMaster);
                }
                
                return Json(pageMaster);
            }

            return BadRequest(ModelState);
        }

        [Authorize(policy: PolicyNames.SiteMasterPagesRead)]
        [HttpPost, Route("/api/masterpages/viewtemplate")]
        public JsonResult ViewTemplate(string templateId)
        {

            var tempModel = _siteContext.Template.PageTemplates.FirstOrDefault(x => x.Id == templateId);
            var model = new PageMasterViewModel();

            model.PreviewPath = "/img/Admin/PageMaster/" + tempModel.PreviewImage;
            model.TemplateId = tempModel.Id;
            model.ViewTemplateTitle = tempModel.Title;

            return Json(model);
        }
    }
}
