using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using AutoMapper.Extensions;

using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Identity;

namespace Angelo.Connect.Web.UI.Components
{
    public class ClientUserRolesEdit : ViewComponent
    {
        private ClientManager _clientManager;
        private UserManager _userManager;
        private SecurityPoolManager _poolManager;

        public ClientUserRolesEdit(ClientManager clientManager, UserManager userManager, SecurityPoolManager poolManager)
        {
            _userManager = userManager;
            _poolManager = poolManager;
            _clientManager = clientManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string clientId, string userId)
        {
            var client = await _clientManager.GetByIdAsync(clientId);
            var allRoles = await _poolManager.GetRolesAsync(client.SecurityPoolId);
            var userRoles = await _poolManager.GetUserRolesAsync(client.SecurityPoolId, userId);

            // TODO - Ensure user's directory is mapped to this site's security pool

            var model = allRoles.Select(x => new UserRoleViewModel
            {
                RoleId = x.Id,
                RoleName = x.Name,
                Selected = userRoles.Any(y => y.Id == x.Id)
            }).ToList();

            ViewData["UserId"] = userId;

            return await Task.Run(() => View(model));
        }
    }
}
