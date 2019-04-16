using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

using Angelo.Connect.Models;
using Angelo.Connect.Services;
using Angelo.Connect.Configuration;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using AutoMapper.Extensions;
using Angelo.Connect.Security;
using Angelo.Identity;

namespace Angelo.Connect.Web.UI.Controllers.Api
{
    public class PagesDataController : SiteControllerBase
    {
        private PageManager _pageManager;
        private SiteManager _siteManager;
        private ResourceManager _resourceManager;
        private AdminContext _adminContext;

        public PagesDataController(
            PageManager pageManager, 
            AdminContext adminContext, 
            SiteManager siteManager, 
            ResourceManager resourceManager, 
            SiteAdminContextAccessor siteContextAccessor, 
            ILogger<SecurityPoolManager> logger) : base(siteContextAccessor, logger)
        {
            _pageManager = pageManager;
            _siteManager = siteManager;
            _resourceManager = resourceManager;
            _adminContext = adminContext;
        }

        [Authorize(policy: PolicyNames.SitePagesRead)]
        [HttpPost, Route("/sys/sites/{tenant}/api/pages/data")]
        public async Task<JsonResult> Data([DataSourceRequest] DataSourceRequest request)
        {
            var siteId = Site.Id;
            var pages = await _pageManager.GetPagesAsync(siteId);
            var model = pages.ProjectTo<PageViewModel>();

            //kendo is not working with null parentIds... setting to empty string
            model.Each(x => {
                x.ParentPageId = x.ParentPageId == null ? "" : x.ParentPageId;
            });

            var result = model
                .AsQueryable()
                .OrderBy(x => x.ParentPageId)
                .ThenBy(x => x.Title)
                .ToTreeDataSourceResult(request, x => x.Id, x => x.ParentPageId, x => x);

            return Json(result);
        }

        [Authorize(policy: PolicyNames.SitePagesCreate)]
        [HttpPost, Route("/sys/sites/{tenant}/api/pages")]
        public async Task<ActionResult> Update(PageViewModel model)
        {
            if (ModelState.IsValid)
            {
                var page = model.ProjectTo<Page>();

                if (page.Id != null)
                    await _pageManager.UpdateAsync(page);
                else
                {
                    var userId = _adminContext.UserContext.Principal.GetUserName();
                    await _pageManager.CreateAsync(page, userId);

                    model.Id = page.Id;
                }

                //return await Edit(model.SiteId, model.Id);
                return Ok(model);
            }

            return BadRequest(ModelState);
        }

        [Authorize(policy: PolicyNames.SitePagesEdit)]
        [HttpPost, Route("/sys/sites/{tenant}/api/pages/{id}/move")]
        public async Task<ActionResult> UpdateParent(string id, [FromForm] string parentId)
        {
           
           var success = await _pageManager.UpdateParentAsync(id, parentId);

           if(success)
                return Ok();

           return BadRequest();
        }

        [Authorize(policy: PolicyNames.SitePagesDelete)]
        [HttpDelete, Route("/sys/sites/{tenant}/api/pages/{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            if (id != null)
            {
                await _pageManager.RemoveAsync(id);
                return Ok();
            }

            return BadRequest(ModelState);
        }

        [Authorize(policy: PolicyNames.SitePagesEdit)]
        [HttpPost, Route("/api/pages/master")]
        public async Task<ActionResult> UpdatePageMaster(PageViewModel model)
        {
            if (model != null && model.Id != null)
            {
                await _pageManager.UpdatePageMasterAsync(model.Id, model.PageMasterId);

                return Ok(model);
            }

            return BadRequest(ModelState);
        }

      
    }
}
