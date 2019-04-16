using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper.Extensions;

using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Common.Mvc.ActionResults;

namespace Angelo.Connect.Web.UI.Components
{
    public class ClientEditForm : ViewComponent
    {
        private ClientManager _clientManager;

        public ClientEditForm(ClientManager clientManager)
        {
            _clientManager = clientManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string clientId = null, bool create = false)
        {
            if (string.IsNullOrEmpty(clientId) && create.Equals(false))
                return new ViewComponentPlaceholder();

            var result = await _clientManager.GetByIdAsync(clientId);
            var model = result.ProjectTo<ClientViewModel>();
            ViewData["Create"] = create;

            return View(model);
        }
    }
}
