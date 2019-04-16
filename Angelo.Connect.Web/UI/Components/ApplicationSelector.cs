using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Menus;
using Angelo.Connect.Services;

namespace Angelo.Connect.Web.UI.Components
{
    public class ApplicationSelector : ViewComponent
    {
        private ClientAdminContext _clientContext;
        private ClientManager _clientManager;

        public ApplicationSelector
        (
            ClientAdminContextAccessor clientContextAccessor, 
            ClientManager clientManager
        )
        {
            _clientContext = clientContextAccessor.GetContext();
            _clientManager = clientManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var apps = await _clientManager.GetProductsAsync(_clientContext.Client.Id);
            var items = new List<SelectListItem>();
            foreach(var item in apps)
            {
                items.Add(new SelectListItem()
                {
                    Text = item.Title,
                    Value = item.Id
                });
            }

            ViewData["ReturnUrl"] = Request.Path.ToUriComponent();
            ViewData["ClientId"] = _clientContext.Client.Id;
            ViewData["AppId"] = _clientContext.Product.AppId;
            ViewData["SelectListItems"] = items;

            return View();
        }
    }
}