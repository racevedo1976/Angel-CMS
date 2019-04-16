using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc.Rendering;
using Angelo.Connect.NavMenu.Services;
using Angelo.Connect.Configuration;
using Angelo.Connect.Data;
using Angelo.Connect.Security;
using Angelo.Connect.Services;
using Angelo.Connect.NavMenu.Models;
using Angelo.Connect.NavMenu.Data;

namespace Angelo.Connect.NavMenu.UI.Components
{
    public class NavMenuWidgetConfigForm : ViewComponent
    {
        SiteContext _siteContext;
        ConnectDbContext _db;
        NavMenuWidgetService _widgetService;
        UserContext _userContext;
        NavigationMenuManager _navMenuManager;

        public NavMenuWidgetConfigForm(SiteContext siteContext, ConnectDbContext db,
            NavMenuWidgetService widgetService, UserContext userContext, NavigationMenuManager navMenuManager)
        {
            _siteContext = siteContext;
            _db = db;
            _widgetService = widgetService;
            _userContext = userContext;
            _navMenuManager = navMenuManager;
        }

        private List<SelectListItem> GetNavMenuSelectList()
        {
            var menus = _navMenuManager.GetNavMenusOfSiteIdAsync(_siteContext.SiteId).Result;
            var list = new List<SelectListItem>();
            foreach(var menu in menus)
            {
                list.Add(new SelectListItem()
                {
                    Value = menu.Id,
                    Text = menu.Title
                });
            }
            return list.OrderBy(x => x.Text).ToList();
        }

        public async Task<IViewComponentResult> InvokeAsync(NavMenuWidget model)
        {
            var viewModel = model.ProjectToViewModel();
            ViewData["siteId"] = _siteContext.SiteId;
            ViewData["navMenuSelectList"] = GetNavMenuSelectList();

            return View(viewModel);
        }
    }
}
