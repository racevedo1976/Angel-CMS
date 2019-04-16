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
using Angelo.Identity;

namespace Angelo.Connect.Web.UI.Components
{
    public class ClientRoleEditViewComponent : ViewComponent
    {
        private ClientManager _clientManager;
        private RoleManager _roleManager;

        public ClientRoleEditViewComponent(ClientManager clientManager, RoleManager roleManager)
        {
            _clientManager = clientManager;
            _roleManager = roleManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string clientId = "", string roleId = "")
        {
            var client = await _clientManager.GetByIdAsync(clientId);
            var role = await _roleManager.GetByIdAsync(roleId);

            var model = new PoolRoleViewModel()
            {
                Id = role.Id,
                Name = role.Name,
                IsDefault = role.IsDefault,
                IsLocked = role.IsLocked,
                PoolId = client.SecurityPoolId
            };

            ViewData["clientId"] = clientId;

            return View(model);
        }
    }
}
