using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper.Extensions;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Models;
using Angelo.Connect.Services;
using Angelo.Connect.Security;
using Angelo.Connect.Security.Services;
using Angelo.Connect.UserConsole;

using Angelo.Connect.Web.UI.UserConsole.Components;
using Angelo.Connect.Web.UI.UserConsole.ViewModels;

namespace Angelo.Connect.Web.UI.UserConsole.Controllers
{
    public class UserPageController : Controller
    {

        private PageManager _pageManager;
        private SiteManager _siteManager;
        private PageSecurityService _pageSecurity;
        private UserPageComponent _userPageComponent;
        private UserConsoleComponentFactory _userComponentFactory;
        private IContextAccessor<UserContext> _userContextAccessor;

        public UserPageController
        (
            PageManager pageManager, 
            PageSecurityService pageSecurity,
            SiteManager siteManager, 
            UserPageComponent userPageComponent,
            UserConsoleComponentFactory userComponentFactory,
            IContextAccessor<UserContext> userContextAccessor
        )
        {
            _pageManager = pageManager;
            _siteManager = siteManager;
            _pageSecurity = pageSecurity;
            _userPageComponent = userPageComponent;
            _userComponentFactory = userComponentFactory;

            _userContextAccessor = userContextAccessor;
        }


        [HttpGet, Route("/sys/console/pages")]
        public async Task<ActionResult> PageListView([FromQuery] string siteId)
        {
            var site = await _siteManager.GetByIdAsync(siteId);
            var model = await _userPageComponent.GetPageListViewModel(site);

            ViewData.Add("UserPageSiteTitle", site.Title);
            ViewData.Add("UserPageSiteId", site.Id);

            return PartialView("/UI/UserConsole/Views/Pages/PageList.cshtml", model);
        }

        [Authorize]
        [HttpGet, Route("/sys/console/pages/{id}")]
        public async Task<ActionResult> PageEditForm(string id)
        {
            var model = await _userPageComponent.GetPageEditViewModel(id);

            return PartialView("/UI/UserConsole/Views/Pages/PageEdit.cshtml", model);
        }

       

        [Authorize]
        [HttpGet, Route("/sys/console/pages/{id}/new")]
        public async Task<ActionResult> PageCreateForm(string id)
        {
            var model = await _userPageComponent.GetPageCreateViewModel(id);
            
            return PartialView("/UI/UserConsole/Views/Pages/PageCreate.cshtml", model);
        }

        [Authorize]
        [HttpPost, Route("/sys/console/pages")]
        public async Task<ActionResult> Update(UserPageViewModel model)
        {
            var userContext = _userContextAccessor.GetContext();
            var userId = userContext.UserId;

            if (ModelState.IsValid)
            {
                var page = model.ProjectTo<Page>();

                if (page.Id != null)
                    await _pageManager.UpdateAsync(page);
                else
                {
                    await _pageManager.CreateAsync(page, userId);

                    model.Id = page.Id;
                }

                return Ok(model);
            }

            return BadRequest(ModelState);
        }


        [Authorize]
        [HttpDelete, Route("/sys/console/pages/{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                await _pageManager.RemoveAsync(id);

                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost, Route("/sys/console/pages/{id}/move")]
        public async Task<ActionResult> MovePage(string id, [FromForm] string parentId)
        {
            var success = await _pageManager.UpdateParentAsync(id, parentId);

            if (success)
                return Ok();

            return BadRequest();
        }


        [Authorize]
        [HttpPost, Route("/sys/console/pages/data")]
        public async Task<JsonResult> Data([DataSourceRequest] DataSourceRequest request, [FromQuery] string site)
        {
            var userContext = _userContextAccessor.GetContext();
            var pages = await _pageSecurity.GetSitePagesOwnedByUser(userContext, site);
            var model = pages.ProjectTo<UserPageViewModel>();

            //kendo is not working with null parentIds... setting to empty string
            model.Each(x => {
                // if parentPageId is null, or points to a page not in our list
                // then set view model parentId to "" so kendo will show at top level
                if(x.ParentPageId == null || !pages.Any(y => y.Id == x.ParentPageId))
                {
                    x.ParentPageId = "";
                }
            });

            var result = model
                .AsQueryable()
                .OrderBy(x => x.ParentPageId)
                .ThenBy(x => x.Title)
                .ToTreeDataSourceResult(request, x => x.Id, x => x.ParentPageId, x => x);

            return Json(result);
        }
    }
}
