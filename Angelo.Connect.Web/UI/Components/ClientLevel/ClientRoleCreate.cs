using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Connect.Models;
using AutoMapper.Extensions;
using Angelo.Common.Mvc.ActionResults;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Angelo.Connect.Web.UI.Components
{
    public class ClientRoleCreateViewComponent : ViewComponent
    {
        private ClientManager _clientManager;

        public ClientRoleCreateViewComponent(ClientManager clientManager)
        {
            _clientManager = clientManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string clientId)
        {
            var client = await _clientManager.GetByIdAsync(clientId);

            var model = new PoolRoleViewModel()
            {
                PoolId = client.SecurityPoolId,
                IsDefault = false,
                IsLocked = false
            };

            ViewData["clientId"] = clientId;

            return View(model);
        }


    }
}
