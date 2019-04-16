using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Configuration;
using Angelo.Connect.Data;
using Angelo.Connect.CoreWidgets.Models;
using Angelo.Connect.CoreWidgets.UI.ViewModels;

namespace Angelo.Connect.CoreWidgets.UI.Components
{
    public class NavBarMenuForm : ViewComponent
    {
        private ConnectDbContext _connectDb;
        private SiteContext _siteContext;
       
        public NavBarMenuForm(SiteContext siteContext, ConnectDbContext connectDb)
        {
            _siteContext = siteContext;
            _connectDb = connectDb;
        }

        public async Task<IViewComponentResult> InvokeAsync(NavBar model)
        {
            var viewModel = new NavBarMenuViewModel
            {
                Id = model.Id,
                NavMenuId = model.NavMenuId
            };

            viewModel.NavMenus = await _connectDb.NavigationMenu
                .Where(x => x.SiteId == _siteContext.SiteId)
                .ToListAsync();

            return View(viewModel);
        }
    }
}
