using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

using Angelo.Connect.Services;
using Angelo.Connect.Security;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Identity;
using AutoMapper.Extensions;

namespace Angelo.Connect.Web.UI.Controllers.Api
{
    public class PagesSecurityRoleDataController : Controller
    {
        private PageSecurityManager _pageManager;
        private SiteManager _siteManager;

        public PagesSecurityRoleDataController(PageSecurityManager pageManager, SiteManager siteManager)
        {
            _pageManager = pageManager;
            _siteManager = siteManager;
        }

        [Authorize(policy: PolicyNames.SitePagesEdit)]
        [HttpPost, Route("/api/pages/security/roles/data")]
        public async Task<JsonResult> Data([DataSourceRequest] DataSourceRequest request, string pageId)
        {
            //var model = new PageSecurityUserViewModel();

            var pageSecurityRoles = await _pageManager.GetPageSecurityRolesAsync(pageId);
            var model = pageSecurityRoles.ProjectTo<PageSecurityRoleViewModel>();

            //var result = model
            //    .AsQueryable()
            //    .OrderBy(x => x.ParentPageId)
            //    .ThenBy(x => x.Title)
            //    .ToTreeDataSourceResult(request, x => x.Id, x => x.ParentPageId, x => x);

            return Json(model.ToDataSourceResult(request));
        }
    }
}
