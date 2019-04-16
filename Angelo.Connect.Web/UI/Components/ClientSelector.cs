using Angelo.Connect.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.Components
{
    public class ClientSelector : ViewComponent
    {
        private ClientManager _clientManager;
        public ClientSelector(ClientManager clientManager)
        {
            _clientManager = clientManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new List<SelectListItem>();

            var clientList = await _clientManager.GetAll();

            foreach (var client in clientList)
            {
                model.Add(new SelectListItem { Value = client.Id, Text = client.Name });
            }

            return View(model);
        }
    }
}